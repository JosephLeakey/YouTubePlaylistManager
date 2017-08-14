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
using System.Collections;
using CefSharp;
using CefSharp.WinForms;
using System.Security.Permissions;
using System.Threading;

namespace YTMP
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [System.Runtime.InteropServices.ComVisible(true)]
    public partial class MainForm : System.Windows.Forms.Form
    {
        private Framework framework;

        private ChromiumWebBrowser player;

        private const string HTML = @"<meta http-equiv=""X-UA-Compatible"" content=""IE=edge""/>
 <html>
  <body style=""margin: 0"">
    <div id=""player""></div>

    <script>
      var tag = document.createElement('script');

      tag.src = ""https://www.youtube.com/iframe_api"";
      var firstScriptTag = document.getElementsByTagName('script')[0];
      firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);
	  
      var player;

      function onYouTubeIframeAPIReady()
      {
        player = new YT.Player('player', {
          height: '100%',
          width: '100%',
          playerVars: {
            autoplay: 1,
            enablejsapi: 1,
            iv_load_policy: 3,
            modestbranding: 1,
            rel: 0
            },
          events: {
            'onReady': onPlayerReady,
            'onStateChange': onPlayerStateChange
            }
        });
      }

      function onPlayerReady(event) {
        event.target.playVideo();
      }

      function onPlayerStateChange(event) {
        if (event.data == YT.PlayerState.ENDED) {
            interface.nextVideo();
        }
      }
    </script>
  </body>
</html>";

        //The menu that will be displayed when a row in the playlist is right-clicked on
        private ContextMenu menu = new ContextMenu();

        //The text to be displayed in the searchbar when it's inactive
        private const string searchBoxText = "Search through the playlist or add a video to it...";

        //Tracks whether or not changes have been made to the current playlist since it was previously exported/imported
        private bool unsaved = false;

        //Tracks the indexes of the playlist entries that users may choose to delete
        private int deletionIndex;

        public DataGridViewRow current;

        public bool autoplay = true;

        public bool shuffle = false;

        public MainForm()
        {
            framework = new Framework();

            InitializeComponent();

            InitializeBrowser();
        }

        private void InitializeBrowser()
        {
            Cef.Initialize(new CefSettings());

            player = new ChromiumWebBrowser(string.Empty);
            player.Size = new Size(480, 270);
            player.Location = new Point(770, 31);
            player.BackColor = Color.Black;
            player.Anchor = (AnchorStyles.Top | AnchorStyles.Right);

            player.LoadHtml(HTML, "http://HTML/");
            player.RegisterJsObject("interface", new JavascriptInterface(this, framework, playlist));

            this.Controls.Add(player);
            player.BringToFront();
        }

        //Executed when the form is loaded
        private void MainForm_Load(object sender, EventArgs e)
        {
            //Display placeholder text within the search-bar
            searchBox.Text = searchBoxText;

            menu.MenuItems.Add(new MenuItem("Delete", new EventHandler(DeleteRow)));

            SetFullView(false);
        }

        private void SetSearchBar(int mode)
        {
            switch (mode)
            {
                default:
                case (0):
                    addButton.Visible = false;
                    searchBox.Size = new Size(playlist.Size.Width, searchBox.Size.Height);

                    searchBox.ForeColor = SystemColors.ControlDark;
                    searchBox.Font = new Font(searchBox.Font, FontStyle.Italic);
                    searchBox.Text = searchBoxText;

                    playlist.Focus();
                    break;
                case (1):
                    addButton.Visible = false;
                    searchBox.Size = new Size(playlist.Size.Width, searchBox.Size.Height);
                    break;
                case (2):
                    searchBox.Size = new Size(playlist.Size.Width - 104, searchBox.Size.Height);
                    addButton.Visible = true;
                    break;
            }
        }

        private void Reset()
        {
            SetFullView(false);

            unsaved = false;

            string search = GetSearchText();

            if (search.Length < 11 || !framework.ReadyToAdd(playlist, search.Substring(search.Length - 11)))
            {
                SetSearchBar(0);
            }
        }

        private string GetSearchText()
        {
            string search = searchBox.Text;

            while (search.Length > 11 && search[search.Length - 1] == char.Parse("/"))
            {
                search = search.Substring(0, search.Length - 1);
            }

            return search;
        }

        private void Playlist_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            playlist.ClearSelection();

            playlist.Rows[e.RowIndex].Selected = true;

            PlayVideo(playlist.SelectedRows[0]);
        }

        public void SetFullView(bool state)
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

        public void PlayVideo(DataGridViewRow row)
        {
            current = row;

            player.GetMainFrame().ExecuteJavaScriptAsync(@"player.loadVideoById(""" + current.Cells[1].Value + @""")");

            if (InvokeRequired)
            {
                BeginInvoke((MethodInvoker)delegate () {
                    UpdateVideoDetails(row);
                    UpdateUIElements();
                });
            }
            else
            {
                UpdateVideoDetails(row);
                UpdateUIElements();
            }
            
            SetFullView(true);
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            string search = GetSearchText();

            if (search.Length > 10 && framework.ReadyToAdd(playlist, search.Substring(search.Length - 11)))
            {
                SetSearchBar(2);

                search = string.Empty;
            }
            else if (searchBox.ForeColor != SystemColors.ControlDark)
            {
                SetSearchBar(1);
            }

            if (search.Length == 0 || searchBox.ForeColor == SystemColors.ControlDark)
            {
                foreach (DataGridViewRow row in playlist.Rows)
                {
                    row.Visible = true;
                }

                return;
            }

            framework.Search(playlist, search);
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            string search = GetSearchText();

            framework.AddVideo(playlist, search.Substring(search.Length - 11));

            SetSearchBar(0);

            unsaved = true;

            //playlist.ClearSelection();
            //playlist.Rows[playlist.RowCount - 1].Selected = true;
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

                if (searchBox.Width < playlist.Width)
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
                SetSearchBar(0);
            }
        }

        private void Export()
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                framework.Export(playlist, saveFileDialog.FileName);

                unsaved = false;
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

            if (loadFileDialog.ShowDialog() == DialogResult.OK)
            {
                framework.Import(playlist, loadFileDialog.FileName);

                Reset();
            }
        }

        private void playlist_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                playlist.ClearSelection();

                playlist.Rows[e.RowIndex].Selected = true;

                menu.Show(this, this.PointToClient(MousePosition));
            }
        }

        private void playlist_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            deletionIndex = playlist.SelectedRows[0].Index;

            for (int c = 1; c < playlist.SelectedRows.Count; c++)
            {
                if (playlist.SelectedRows[c].Index < deletionIndex)
                {
                    deletionIndex = playlist.SelectedRows[c].Index;
                }
            }

            if (player.Visible)
            {
                foreach (DataGridViewRow row in playlist.SelectedRows)
                {
                    if (row.Cells[1].Value.ToString() == current.Cells[1].Value.ToString())
                    {
                        UpdateVideoNameTag(0);

                        return;
                    }
                }
            }
        }

        private void playlist_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        { 
            for (int c = deletionIndex; c < playlist.RowCount; c++)
            {
                playlist.Rows[c].Cells[0].Value = c + 1;
            }

            unsaved = true;

            if (current == null)
            {
                return;
            }

            if (player.Visible)
            {
                foreach (DataGridViewRow row in playlist.Rows)
                {
                    if (row.Cells[1].Value.ToString() == current.Cells[1].Value.ToString())
                    {
                        current = row;

                        UpdateVideoNameTag((int)current.Cells[0].Value);
                    }
                }
            }
        }

        private void DeleteRow(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in playlist.SelectedRows)
            {
                playlist.Rows.Remove(row);
            }
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

            Reset();
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            DataGridViewRow nextVideo = framework.Advance(playlist, current);

            if (nextVideo != null)
            {
                PlayVideo(nextVideo);
            }
        }

        private void previousButton_Click(object sender, EventArgs e)
        {
            DataGridViewRow previousVideo = framework.Retreat(playlist, current);

            if (previousVideo != null)
            {
                PlayVideo(previousVideo);
            }
        }

        private void autoPlayToggleButton_Click(object sender, EventArgs e)
        {
            autoplay = !autoplay;

            if (autoplay)
            {
                autoPlayToggleButton.Text = "Auto-Play: ON";
            }
            else
            {
                autoPlayToggleButton.Text = "Auto-Play: OFF";
            }
        }

        private void videoNameLabel_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = framework.FindCurrentVideo(playlist, current);

            if (row != null)
            {
                playlist.ClearSelection();

                row.Selected = true;
            }
        }

        private void UpdateVideoName(string name, int number)
        {
            if (number > 0)
            {
                videoNameLabel.Text = "[" + number + "] " + name;
            }
            else
            {
                videoNameLabel.Text = "[-] " + name;
            }
        }

        private void UpdateVideoNameTag(int number)
        {
            if (number > 0)
            {
                videoNameLabel.Text = "[" + number + "] " + videoNameLabel.Text.Substring(videoNameLabel.Text.IndexOf("]") + 2);
            }
            else
            {
                videoNameLabel.Text = "[-] " + videoNameLabel.Text.Substring(videoNameLabel.Text.IndexOf("]") + 2);
            }
        }

        private void UpdateVideoDetails(DataGridViewRow entry)
        {
            videoNameLabel.Text = "[" + (int)entry.Cells[0].Value + "] " + (string)entry.Cells[2].Value;
            videoUploaderLabel.Text = "Uploaded by " + (string)entry.Cells[3].Value;
            videoDescriptionBox.Text = (string)entry.Cells[4].Value;
        }

        private void UpdateUIElements()
        {
            videoUploaderLabel.Location = new Point(videoUploaderLabel.Location.X, videoNameLabel.Location.Y + videoNameLabel.Size.Height);
            videoDescriptionBox.Location = new Point(videoDescriptionBox.Location.X, videoUploaderLabel.Location.Y + videoUploaderLabel.Size.Height + 15);
            videoDescriptionBox.Size = new Size(videoDescriptionBox.Size.Width, previousButton.Location.Y - videoDescriptionBox.Location.Y - 5);
        }
    }

    public class JavascriptInterface
    {
        private MainForm form;
        private Framework framework;
        private DataGridView playlist;

        public JavascriptInterface(MainForm form, Framework framework, DataGridView playlist)
        {
            this.form = form;
            this.framework = framework;
            this.playlist = playlist;
        }

        public void NextVideo()
        {
            if (form.autoplay)
            {
                form.PlayVideo(framework.Advance(playlist, form.current));
            }
        }
    }
}
