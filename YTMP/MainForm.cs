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
        private int deletionIndex = -1;

        private string preservationID;

        private int visibleCount;

        private bool online;

        public Dictionary<string, object[]> playlist = new Dictionary<string, object[]>();

        public Dictionary<int, string> listing = new Dictionary<int, string>();

        public int current;

        public bool autoplay = true;

        public bool shuffle = false;

        //private Stack<string> shuffleStack = new Stack<string>();

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
            player.RegisterJsObject("interface", new JavascriptInterface(this, framework));

            this.Controls.Add(player);
            player.BringToFront();
        }

        //Executed when the form is loaded
        private void MainForm_Load(object sender, EventArgs e)
        {
            //Display placeholder text within the search-bar
            searchBox.Text = searchBoxText;

            menu.MenuItems.Add(new MenuItem("Delete", new EventHandler(DeleteMenuItemEventHandler)));

            SetFullView(false);
        }

        private void SetSearchBar(int mode)
        {
            switch (mode)
            {
                default:
                case (0):
                    addButton.Visible = false;
                    searchBox.Size = new Size(playlistGrid.Size.Width, searchBox.Size.Height);

                    searchBox.ForeColor = SystemColors.ControlDark;
                    searchBox.Font = new Font(searchBox.Font, FontStyle.Italic);
                    searchBox.Text = searchBoxText;

                    playlistGrid.Focus();
                    break;
                case (1):
                    addButton.Visible = false;
                    searchBox.Size = new Size(playlistGrid.Size.Width, searchBox.Size.Height);
                    break;
                case (2):
                    searchBox.Size = new Size(playlistGrid.Size.Width - 104, searchBox.Size.Height);
                    addButton.Visible = true;
                    break;
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
            playlistGrid.ClearSelection();

            playlistGrid.Rows[e.RowIndex].Selected = true;

            if (!CheckYouTube(false, true))
            {
                SetFullView(false);

                return;
            }

            current = e.RowIndex;

            PlayVideo(e.RowIndex);
        }

        public void SetFullView(bool state)
        {
            if (state && this.MinimumSize.Width < 1280)
            {
                searchBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
                addButton.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
                playlistGrid.Anchor = (AnchorStyles.Top | AnchorStyles.Left);

                this.Size = new Size(this.Width + 494, this.Height);
                this.MinimumSize = new Size(1280, 720);

                searchBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
                addButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                playlistGrid.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom);

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
                try { player.GetMainFrame().ExecuteJavaScriptAsync("player.stopVideo()"); } catch { }

                player.Visible = false;
                videoNameLabel.Visible = false;
                videoUploaderLabel.Visible = false;
                videoDescriptionBox.Visible = false;
                previousButton.Visible = false;
                nextButton.Visible = false;
                minMaxToggleButton.Visible = false;

                searchBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
                addButton.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
                playlistGrid.Anchor = (AnchorStyles.Top | AnchorStyles.Left);

                this.MinimumSize = new Size(786, 720);
                this.Size = new Size(this.Width - 494, this.Height);

                searchBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
                addButton.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                playlistGrid.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom);
            }
        }

        private void ResetApplication()
        {
            SetFullView(false);

            exportFileButton.Enabled = false;

            string search = GetSearchText();

            if (framework.VideoExists(search.Substring(search.Length - 11)))
            {
                SetSearchBar(2);
            }
            else
            {
                SetSearchBar(0);
            }

            unsaved = false;
        }

        public void PlayVideo(int index)
        {
            if (!framework.VideoExists(listing[current]))
            {
                MessageBox.Show("This video could not be loaded.\n\nPlease check your internet connection\nand make sure that the video still exists.", "Unable to Load Video", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            player.GetMainFrame().ExecuteJavaScriptAsync(@"player.loadVideoById(""" + listing[current] + @""")");

            if (InvokeRequired)
            {
                BeginInvoke((MethodInvoker)delegate () {
                    UpdateVideoDetails(current);
                    UpdateUIElements();
                });
            }
            else
            {
                UpdateVideoDetails(current);
                UpdateUIElements();
            }
            
            SetFullView(true);
        }

        private void Search()
        {
            string search = GetSearchText();

            if (search.Length > 10 && !playlist.ContainsKey(search.Substring(search.Length - 11)) && framework.VideoExists(search.Substring(search.Length - 11)))
            {
                SetSearchBar(2);

                search = string.Empty;
            }
            else if (searchBox.ForeColor == SystemColors.ControlDark)
            {
                search = string.Empty;
            }
            else
            {
                SetSearchBar(1);
            }

            string[] results = framework.Search(playlist, listing, search);

            visibleCount = results.Length;

            bool changed = false;
            
            foreach (DataGridViewRow row in playlistGrid.Rows)
            {
                if (row.Visible != results.Contains(row.Cells[1].Value.ToString()))
                {
                    row.Visible = !row.Visible;

                    changed = true;
                }
            }

            if (changed)
            {
                playlistGrid.ClearSelection();
            }
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            Search();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            string ID = GetSearchText();
            ID = ID.Substring(ID.Length - 11);

            object[] entry = framework.GetVideoDetails(ID);

            playlist[ID] = (object[])entry[1];

            AddVideoToGrid(ID, (object[])entry[1], true);

            newPlaylistButton.Enabled = true;

            exportFileButton.Enabled = true;

            SetSearchBar(0);

            visibleCount += 1;

            unsaved = true;
        }

        private void AddVideoToGrid(string ID, object[] details, bool format)
        {
            if (format)
            {
                int time = (int)details[3];
                string timeString = string.Empty;

                if (time / 86400 > 0)
                {
                    timeString += string.Format("{0:00}", time / 86400) + ":";

                    time = time % 86400;
                }

                if (time / 3600 > 0)
                {
                    timeString += string.Format("{0:00}", time / 3600) + ":";

                    time = time % 3600;
                }

                if (time / 60 > 0)
                {
                    timeString += string.Format("{0:00}", time / 60) + ":";

                    time = time % 60;
                }

                if (time > 0)
                {
                    timeString += string.Format("{0:00}", time);
                }

                playlistGrid.Rows.Add(playlistGrid.RowCount + 1, ID, details[0], details[1], details[2], timeString);
            }
            else
            {
                playlistGrid.Rows.Add(playlistGrid.RowCount + 1, ID, details[0], details[1], details[2], details[3]);
            }

            listing[playlistGrid.RowCount - 1] = ID;
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
                player.Location = new Point(playlistGrid.Location.X + playlistGrid.Width + 14, player.Location.Y);
                player.Anchor = (AnchorStyles.Top | AnchorStyles.Right);

                foreach (Control C in Controls)
                {
                    if (C != player && C != menuStrip && C != addButton)
                    {
                        C.Visible = true;
                    }
                }

                playlistOptionsButton.Visible = true;

                if (searchBox.Width < playlistGrid.Width)
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
                framework.Export(playlist, listing, saveFileDialog.FileName);

                unsaved = false;
            }
        }

        private void exportFileButton_Click(object sender, EventArgs e)
        {
            Export();
        }

        private void importFileButton_Click(object sender, EventArgs e)
        {
            if (unsaved && exportFileButton.Enabled)
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
                online = framework.Import(playlist, loadFileDialog.FileName);

                playlistGrid.Rows.Clear();

                foreach (string entry in playlist.Keys)
                {
                    AddVideoToGrid(entry, playlist[entry], online);

                    listing[playlistGrid.RowCount - 1] = entry;
                }

                visibleCount = playlistGrid.RowCount;

                newPlaylistButton.Enabled = true;

                ResetApplication();
            }
        }

        private void playlist_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                playlistGrid.Rows[e.RowIndex].Selected = true;

                menu.Show(this, this.PointToClient(MousePosition));
            }
        }

        private void playlist_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            DeletionPreparation();
        }

        private void DeleteMenuItemEventHandler(object sender, EventArgs e)
        {
            DeletionPreparation();

            foreach (DataGridViewRow row in playlistGrid.SelectedRows)
            {
                playlistGrid.Rows.Remove(row);
            }
        }

        private void DeletionPreparation()
        {
            if (deletionIndex == -1)
            {
                deletionIndex = playlistGrid.SelectedRows[0].Index;

                foreach (DataGridViewRow row in playlistGrid.SelectedRows)
                {
                    playlist.Remove(listing[row.Index]);
                    listing.Remove(row.Index);

                    if (row.Index < deletionIndex) { deletionIndex = row.Index; }
                }

                visibleCount -= playlistGrid.SelectedRows.Count;

                if (player.Visible)
                {
                    if (listing.ContainsKey(current))
                    {
                        preservationID = listing[current];
                    }
                    else
                    {
                        preservationID = null;

                        UpdateVideoNameTag(0);
                    }
                }
            }
        }

        private void playlist_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (playlistGrid.SelectedRows.Count == 0)
            {
                for (int c = 0; c < playlistGrid.RowCount; c++)
                {
                    if (c >= deletionIndex)
                    {
                        playlistGrid.Rows[c].Cells[0].Value = c + 1;

                        listing[c] = playlistGrid.Rows[c].Cells[1].Value.ToString();
                    }

                    if (player.Visible && preservationID != null && playlistGrid.Rows[c].Cells[1].Value.ToString() == preservationID)
                    {
                        current = c;

                        UpdateVideoNameTag(current);

                        preservationID = null;
                    }
                }

                deletionIndex = -1;

                if (visibleCount == 0)
                {
                    string search = GetSearchText();

                    if (search.Length < 11 || !framework.VideoExists(search.Substring(search.Length - 11)))
                    {
                        SetSearchBar(0);
                    }

                    Search();
                }

                newPlaylistButton.Enabled = (playlistGrid.RowCount > 0 || player.Visible);

                exportFileButton.Enabled = (playlistGrid.RowCount > 0);

                unsaved = true;
            }
        }

        private void newPlaylistButton_Click(object sender, EventArgs e)
        {
            if (unsaved && exportFileButton.Enabled)
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

            playlistGrid.Rows.Clear();

            visibleCount = 0;

            ResetApplication();
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            if (framework.NextVideo(playlist, listing, ref current) != null)
            {
                PlayVideo(current);
            }
        }

        private void previousButton_Click(object sender, EventArgs e)
        {
            if (framework.PreviousVideo(playlist, listing, ref current) != null)
            {
                PlayVideo(current);
            }
        }

        private void videoNameLabel_Click(object sender, EventArgs e)
        {
            if (videoNameLabel.Text.Substring(0, 3) != "[-]")
            {
                playlistGrid.ClearSelection();

                playlistGrid.Rows[current].Selected = true;
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

        private void UpdateVideoDetails(int index)
        {
            object[] details = playlist[listing[index]];

            videoNameLabel.Text = "[" + (index + 1) + "] " + details[0];
            videoUploaderLabel.Text = "Uploaded by " + details[1];
            videoDescriptionBox.Text = (string)details[2];
        }

        private void UpdateUIElements()
        {
            videoUploaderLabel.Location = new Point(videoUploaderLabel.Location.X, videoNameLabel.Location.Y + videoNameLabel.Size.Height);
            videoDescriptionBox.Location = new Point(videoDescriptionBox.Location.X, videoUploaderLabel.Location.Y + videoUploaderLabel.Size.Height + 15);
            videoDescriptionBox.Size = new Size(videoDescriptionBox.Size.Width, previousButton.Location.Y - videoDescriptionBox.Location.Y - 5);
        }

        private void importURLButton_Click(object sender, EventArgs e)
        {
            if (!CheckYouTube(true, true))
            {
                return;
            }

            ImportForm import = new ImportForm(framework);

            import.ShowDialog();
        }

        public bool CheckYouTube(bool refresh, bool message)
        {
            online = framework.YouTubeAvailable(refresh);

            if (!online)
            {
                if (message)
                {
                    MessageBox.Show("YouTube could not be reached\n\nPlease check your internet connection.", "Unable to Contact YouTube", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return false;
            }

            return true;
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

        private void shuffleToggleButton_Click(object sender, EventArgs e)
        {
            shuffle = !shuffle;

            if (shuffle)
            {
                shuffleToggleButton.Text = "Shuffle: ON";
            }
            else
            {
                //shuffleStack.Clear();

                shuffleToggleButton.Text = "Shuffle: OFF";
            }
        }
    }

    public class JavascriptInterface
    {
        private MainForm form;
        private Framework framework;

        public JavascriptInterface(MainForm form, Framework framework)
        {
            this.form = form;
            this.framework = framework;
        }

        public void NextVideo()
        {
            if (form.autoplay)
            {
                if (!form.CheckYouTube(false, false))
                {
                    form.SetFullView(false);

                    return;
                }
                
                if ((!form.shuffle && framework.NextVideo(form.playlist, form.listing, ref form.current) != null) || (form.shuffle && framework.RandomVideo(form.playlist, form.listing, ref form.current) != null))
                {
                    form.PlayVideo(form.current);
                }
            }
        }
    }
}
