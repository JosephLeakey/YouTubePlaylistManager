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

        private int count = 0;

        private bool unsaved = false;

        const string searchBoxText = "Search through the playlist or add a video to it...";

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            searchBox.Text = searchBoxText;

            menu.MenuItems.Add(new MenuItem("Delete", new EventHandler(DeleteRow)));
        }

        private void YTPlay(string ID)
        {
            player.Navigate("http://www.youtube.com/v/" + ID);
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

        private bool Exists(string ID)
        {
            for (int i = 0; i < playlist.RowCount; i++)
            {
                if (playlist.Rows[i].Cells[1].Value.ToString() == ID)
                {
                    return true;
                }
            }

            return false;
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
                            SW.WriteLine(playlist.Rows[i].Cells[1].Value.ToString());
                        }
                        else
                        {
                            SW.Write(playlist.Rows[i].Cells[1].Value.ToString());
                        }
                    }
                }

                unsaved = false;
            }
        }

        private void DeleteRow(object sender, EventArgs e)
        {
            for (int i = playlist.SelectedRows[0].Index; i < playlist.RowCount; i++)
            {
                playlist.Rows[i].Cells[0].Value = (int)playlist.Rows[i].Cells[0].Value - 1;
            }

            playlist.Rows.Remove(playlist.SelectedRows[0]);

            count -= 1;
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            if (searchBox.Text.Length > 10 && !Exists(searchBox.Text.Substring(searchBox.Text.Length - 11)))
            {
                DJSON = GetDJSON(searchBox.Text.Substring(searchBox.Text.Length - 11));

                if (DJSON != null)
                {
                    searchBox.Size = new Size(playlist.Size.Width - 104, searchBox.Size.Height);
                    addButton.Visible = true;

                    return;
                }
            }

            DJSON = null;
           
            addButton.Visible = false;
            searchBox.Size = new Size(playlist.Size.Width, searchBox.Size.Height);
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            count += 1;

            playlist.Rows.Add(count, searchBox.Text.Substring(searchBox.Text.IndexOf("watch?v=") + 8), DJSON["title"], DJSON["author_name"]);

            searchBox.Clear();

            unsaved = true;
        }

        private void Playlist_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            YTPlay(playlist.CurrentRow.Cells[1].Value.ToString());
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

                if (DJSON != null)
                {
                    addButton.Visible = true;
                }

                minMaxToggleButton.Text = "Maximise Player";
            }
        }

        private void SearchBox_Click(object sender, EventArgs e)
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
                if (MessageBox.Show("Your current playlist hasn't been saved.\nWould you like to save it before you open another one?", "Unsaved Playlist", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    Export();
                }
            }
            
            if (loadFileDialog.ShowDialog() == DialogResult.OK)
            {

            }
        }

        private void playlist_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                playlist.Rows[e.RowIndex].Selected = true;

                menu.Show(this, this.PointToClient(MousePosition));
            }
        }
    }
}
