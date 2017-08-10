using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web.Script.Serialization;

namespace YTMP
{
    public partial class MainForm : System.Windows.Forms.Form
    {
        private WebClient WC = new WebClient();

        private JavaScriptSerializer Serializer = new JavaScriptSerializer();

        private Dictionary<String, object> DJSON;

        private ContextMenu menu = new ContextMenu();

        private bool unsaved = false;

        const string searchBoxText = "Search through the playlist or add a video to it...";

        private int i;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            searchBox.Text = searchBoxText;

            menu.MenuItems.Add(new MenuItem("Delete", new EventHandler(DeleteRow)));

            SetFullView(false);
        }

        private Dictionary<String, object> GetDJSON(string ID)
        {
            using (WC)
            {
                try
                {
                    string JSON = WC.DownloadString("https://www.youtube.com/oembed?format=json&url=https://www.youtube.com/watch?v=" + ID);

                    return Serializer.Deserialize<Dictionary<String, object>>(JSON);
                }
                catch (WebException WE)
                {
                    return null;
                }
            }
        }

        private int Exists(string ID)
        {
            foreach (DataGridViewRow row in playlist.Rows)
            {
                if (row.Cells[1].Value.ToString().ToLower() == ID.ToLower())
                {
                    return row.Index;
                }
            }

            return -1;
        }

        private void Export()
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (System.IO.StreamWriter SW = new System.IO.StreamWriter(saveFileDialog.FileName))
                {
                    for (int i = 0; i < playlist.RowCount; i++)
                    {
                        if (i < playlist.RowCount - 1)
                        {
                            SW.WriteLine(playlist.Rows[i].Cells[1].Value.ToString() + " - [" + playlist.Rows[i].Cells[3].Value.ToString() + "] " + playlist.Rows[i].Cells[2].Value.ToString());
                        }
                        else
                        {
                            SW.Write(playlist.Rows[i].Cells[1].Value.ToString() + " - [" + playlist.Rows[i].Cells[3].Value.ToString() + "] " + playlist.Rows[i].Cells[2].Value.ToString());
                        }
                    }
                }

                unsaved = false;
            }
        }

        private void Import()
        {
            if (loadFileDialog.ShowDialog() == DialogResult.OK)
            {
                string ID;

                playlist.Rows.Clear();

                using (System.IO.StreamReader SR = new System.IO.StreamReader(loadFileDialog.FileName))
                {
                    while ((ID = SR.ReadLine()) != null) {
                        if (ID.Length > 10)
                        {
                            ID = ID.Substring(0, 11);

                            DJSON = GetDJSON(ID);

                            if (DJSON != null)
                            {
                                playlist.Rows.Add(playlist.RowCount + 1, ID, DJSON["title"], DJSON["author_name"]);
                            }
                        }
                    }
                }

                DJSON = null;
            }
        }

        private void Playlist_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            player.Navigate("http://www.youtube.com/v/" + playlist.SelectedRows[0].Cells[1].Value.ToString());

            videoNameLabel.Text = playlist.SelectedRows[0].Cells[2].Value.ToString();
            videoUploaderLabel.Text = "Uploaded by " + playlist.SelectedRows[0].Cells[3].Value.ToString();
            videoUploaderLabel.Location = new Point(videoUploaderLabel.Location.X, videoNameLabel.Location.Y + videoNameLabel.Size.Height);
            videoDescriptionBox.Location = new Point(videoDescriptionBox.Location.X, videoUploaderLabel.Location.Y + videoUploaderLabel.Size.Height + 15);
            videoDescriptionBox.Size = new Size(videoDescriptionBox.Size.Width, previousButton.Location.Y - videoDescriptionBox.Location.Y - 5);

            SetFullView(true);
        }

        private void SetFullView(bool state)
        {
            if (state && this.MinimumSize.Width < 1280)
            {
                searchBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
                addButton.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
                playlist.Anchor = (AnchorStyles.Top | AnchorStyles.Left);

                this.Size = new Size(this.Width + 494, this.Height);
                this.MinimumSize = new Size(1280, 720);

                searchBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
                addButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                playlist.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom);

                player.Visible = true;
                videoNameLabel.Visible = true;
                videoUploaderLabel.Visible = true;
                videoDescriptionBox.Visible = true;
                previousButton.Visible = true;
                nextButton.Visible = true;
                minMaxToggleButton.Visible = true;
            }
            else if (!state && this.MinimumSize.Width == 1280)
            {
                player.Visible = false;
                player.Stop();

                videoNameLabel.Visible = false;
                videoUploaderLabel.Visible = false;
                videoDescriptionBox.Visible = false;
                previousButton.Visible = false;
                nextButton.Visible = false;
                minMaxToggleButton.Visible = false;

                searchBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
                addButton.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
                playlist.Anchor = (AnchorStyles.Top | AnchorStyles.Left);

                this.MinimumSize = new Size(786, 720);
                this.Size = new Size(this.Width - 494, this.Height);

                searchBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
                addButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                playlist.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom);
            }
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            if (searchBox.Text.Length == 0 || searchBox.ForeColor == SystemColors.ControlDark)
            {
                foreach (DataGridViewRow row in playlist.Rows)
                {
                    row.Visible = true;
                }

                return;
            }

            foreach (DataGridViewRow row in playlist.Rows)
            {
                row.Visible = false;
            }

            if (searchBox.Text.Length > 10)
            {
                int row = Exists(searchBox.Text.Substring(searchBox.Text.Length - 11));

                if (row == -1)
                {
                    DJSON = GetDJSON(searchBox.Text.Substring(searchBox.Text.Length - 11));

                    if (DJSON != null)
                    {
                        searchBox.Size = new Size(playlist.Size.Width - 104, searchBox.Size.Height);
                        addButton.Visible = true;

                        return;
                    }
                }
                else
                {
                    playlist.Rows[row].Visible = true;
                }
            }

            DJSON = null;
           
            addButton.Visible = false;

            if (searchBox.Width < playlist.Size.Width)
            {
                searchBox.Size = new Size(playlist.Size.Width, searchBox.Size.Height);
            }

            for (int c = 0; c < playlist.RowCount; c++)
            {
                for (int d = 0; d < playlist.ColumnCount - 1; d++)
                {
                    if (d != 1 && playlist.Rows[c].Cells[d].Value.ToString().ToLower().Contains(searchBox.Text.ToLower())) {
                        playlist.Rows[c].Visible = true;

                        break;
                    }
                }
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            playlist.Rows.Add(playlist.RowCount + 1, searchBox.Text.Substring(searchBox.Text.IndexOf("watch?v=") + 8), DJSON["title"], DJSON["author_name"]);

            searchBox.Clear();

            unsaved = true;
        }

        private void MinMaxToggleButton_Click(object sender, EventArgs e)
        {
            if (player.Dock == DockStyle.None)
            {
                foreach (Control C in Controls)
                {
                    if (C != player && C != menuStrip)
                    {
                        C.Visible = false;
                    }
                }

                playlistOptionsButton.Visible = false;

                player.Dock = DockStyle.Fill;

                minMaxToggleButton.Text = "Minimise Player";
            }
            else
            {
                player.Dock = DockStyle.None;
                player.Location = new Point(playlist.Location.X + playlist.Width + 14, player.Location.Y);
                player.Anchor = (AnchorStyles.Top | AnchorStyles.Right);

                foreach (Control C in Controls)
                {
                    if (C != player && C != menuStrip && C != addButton)
                    {
                        C.Visible = true;
                    }
                }

                playlistOptionsButton.Visible = true;

                if (DJSON != null)
                {
                    addButton.Visible = true;
                }

                minMaxToggleButton.Text = "Maximise Player";
            }
        }

        private void searchBox_Enter(object sender, EventArgs e)
        {
            if (searchBox.ForeColor == SystemColors.ControlDark)
            {
                searchBox.Clear();
                searchBox.Font = new Font(searchBox.Font, FontStyle.Regular);
                searchBox.ForeColor = SystemColors.WindowText;
            }
        }

        private void SearchBox_Leave(object sender, EventArgs e)
        {
            if (searchBox.TextLength == 0)
            {
                searchBox.ForeColor = SystemColors.ControlDark;
                searchBox.Font = new Font(searchBox.Font, FontStyle.Italic);
                searchBox.Text = searchBoxText;
            }
        }

        private void exportFileButton_Click(object sender, EventArgs e)
        {
            Export();
        }

        private void importFileButton_Click(object sender, EventArgs e)
        {
            if (unsaved)
            {
                switch (MessageBox.Show("Your current playlist hasn't been saved.\nWould you like to save it before you open another one?", "Unsaved Playlist", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning))
                {
                    case (DialogResult.Yes):
                        Export();
                        break;
                    case (DialogResult.Cancel):
                        return;
                }
            }

            Import();
        }

        private void playlist_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                playlist.Rows[e.RowIndex].Selected = true;

                menu.Show(this, this.PointToClient(MousePosition));
            }
        }

        private void playlist_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            i = playlist.SelectedRows[0].Index;

            for (int c = 1; c < playlist.SelectedRows.Count; c++)
            {
                if (playlist.SelectedRows[c].Index < i)
                {
                    i = playlist.SelectedRows[c].Index;
                }
            }
        }

        private void playlist_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            for (int c = i; c < playlist.RowCount; c++)
            {
                playlist.Rows[c].Cells[0].Value = playlist.Rows[c].Index + 1;
            }

            unsaved = true;
        }

        private void DeleteRow(object sender, EventArgs e)
        {
            SendKeys.Send("{DEL}");
        }

        private void newPlaylistButton_Click(object sender, EventArgs e)
        {
            if (unsaved)
            {
                switch (MessageBox.Show("Your current playlist hasn't been saved.\nWould you like to save it before you make a new one?", "Unsaved Playlist", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning))
                {
                    case (DialogResult.Yes):
                        Export();
                        break;
                    case (DialogResult.Cancel):
                        return;
                }
            }

            playlist.Rows.Clear();

            searchBox.Clear();

            unsaved = false;
        }
    }
}
