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
        //The API key used to access YouTube's servers
        private const string API = "AIzaSyBMUx0bkglse9SvI2x69YAQZjARE3jsZG0";

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

        private ChromiumWebBrowser player;

        //Enables the program to download JSON files from YouTube's servers
        private WebClient WC = new WebClient();

        //De-serializes JSON files into dictionaries
        private JavaScriptSerializer serializer = new JavaScriptSerializer();

        //The menu that will be displayed when a row in the playlist is right-clicked on
        private ContextMenu menu = new ContextMenu();

        //Tracks whether or not changes have been made to the current playlist since it was previously exported/imported
        private bool unsaved = false;

        //The text to be displayed in the searchbar when it's inactive
        private const string searchBoxText = "Search through the playlist or add a video to it...";

        //Tracks the indexes of the playlist entries that users may choose to delete
        private int deletionIndex;

        private int currentRow;

        private string currentID;

        public bool autoplay = true;

        public bool shuffle = false;

        private List<string> shuffleStack = new List<string>();

        public MainForm()
        {
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
            player.RegisterJsObject("interface", new JavascriptInterface(this));

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

        private Dictionary<String, object> GetDJSON(string ID)
        {
            using (WC)
            {
                try
                {
                    string JSON = WC.DownloadString("https://www.googleapis.com/youtube/v3/videos?key=AIzaSyBMUx0bkglse9SvI2x69YAQZjARE3jsZG0&part=snippet,contentDetails&id=" + ID);

                    return serializer.Deserialize<Dictionary<String, object>>(JSON);
                }
                catch (WebException WE)
                {
                    return null;
                }
            }
        }

        private string[] GetVideoDetails(Dictionary<String, object> DJSON)
        {
            string[] details = new string[4];

            string time = "";

            dynamic partial;

            Dictionary<String, object> auxiliary;

            partial = (ArrayList)DJSON["items"];
            partial = partial[0];

            auxiliary = partial["contentDetails"];

            partial = (Dictionary<String, object>)partial["snippet"];

            details[0] = (string)partial["title"];
            details[1] = (string)partial["channelTitle"];
            details[2] = (string)partial["description"];
            details[2] = details[2].Replace("\n", "\r\n");

            details[3] = (string)auxiliary["duration"];
            details[3] = details[3].Substring(1).ToLower();

            if (details[3].Contains("d"))
            {
                time += String.Format("{0:00}", int.Parse(details[3].Substring(0, details[3].IndexOf("d")))) + " : ";
            }

            details[3] = details[3].Substring(details[3].IndexOf("t") + 1);

            if (details[3].Contains("h"))
            {
                time += String.Format("{0:00}", int.Parse(details[3].Substring(0, details[3].IndexOf("h")))) + " : ";

                details[3] = details[3].Substring(details[3].IndexOf("h") + 1);
            }

            if (details[3].Contains("m"))
            {
                time += String.Format("{0:00}", int.Parse(details[3].Substring(0, details[3].IndexOf("m")))) + " : ";

                details[3] = details[3].Substring(details[3].IndexOf("m") + 1);
            }
            else
            {
                time += "00 : ";
            }

            if (details[3].Contains("s"))
            {
                time += String.Format("{0:00}", int.Parse(details[3].Substring(0, details[3].IndexOf("s"))));
            }
            else
            {
                time += "00";
            }

            details[3] = time;

            return details;
        }

        private string[] GetVideoDetails(string ID)
        {
            return GetVideoDetails(GetDJSON(ID));
        }

        private object[] GetPlaylistEntryDetails(int index)
        {
            if (index >= 0 && index < playlist.RowCount)
            {
                return new object[] { playlist.Rows[index].Cells[0].Value,
                    playlist.Rows[index].Cells[1].Value,
                    playlist.Rows[index].Cells[2].Value,
                    playlist.Rows[index].Cells[3].Value,
                    playlist.Rows[index].Cells[4].Value };
            }
            else
            {
                return null;
            }
        }

        private int EntryExists(string ID)
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

        private bool VideoExists(string ID)
        {
            using (WC)
            {
                try
                {
                    string JSON = WC.DownloadString("https://www.youtube.com/oembed?format=json&url=https://www.youtube.com/watch?v=" + ID);

                    return true;
                }
                catch (WebException WE)
                {
                    return false;
                }
            }
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

                SetFullView(false);

                using (System.IO.StreamReader SR = new System.IO.StreamReader(loadFileDialog.FileName))
                {
                    while ((ID = SR.ReadLine()) != null)
                    {
                        if (ID.Length > 10)
                        {
                            ID = ID.Substring(0, 11);

                            if (VideoExists(ID))
                            {
                                string[] videoDetails = GetVideoDetails(ID);

                                playlist.Rows.Add(playlist.RowCount + 1, ID, videoDetails[0], videoDetails[1], videoDetails[2], videoDetails[3]);
                            }
                        }
                    }
                }
            }
        }

        private void ResetSearchBar()
        {
            searchBox.ForeColor = SystemColors.ControlDark;
            searchBox.Font = new Font(searchBox.Font, FontStyle.Italic);
            searchBox.Text = searchBoxText;

            playlist.Focus();
        }

        private void Playlist_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            PlayVideo(playlist.SelectedRows[0].Index);
        }

        private void PlayVideo(int rowIndex)
        {
            object[] details = GetPlaylistEntryDetails(rowIndex);

            currentRow = (int)details[0];
            currentID = (string)details[1];

            player.GetMainFrame().ExecuteJavaScriptAsync(@"player.loadVideoById(""" + currentID + @""")");

            if (InvokeRequired)
            {
                BeginInvoke((MethodInvoker)delegate () {
                    videoNameLabel.Text = "[" + (int)details[0] + "] " + (string)details[2];
                    videoUploaderLabel.Text = "Uploaded by " + (string)details[3];
                    videoDescriptionBox.Text = (string)details[4];
                });
            }
            else
            {
                videoNameLabel.Text = "[" + (int)details[0] + "] " + (string)details[2];
                videoUploaderLabel.Text = "Uploaded by " + (string)details[3];
                videoDescriptionBox.Text = (string)details[4];
            }

            videoUploaderLabel.Location = new Point(videoUploaderLabel.Location.X, videoNameLabel.Location.Y + videoNameLabel.Size.Height);
            videoDescriptionBox.Location = new Point(videoDescriptionBox.Location.X, videoUploaderLabel.Location.Y + videoUploaderLabel.Size.Height + 15);
            videoDescriptionBox.Size = new Size(videoDescriptionBox.Size.Width, previousButton.Location.Y - videoDescriptionBox.Location.Y - 5);

            SetFullView(true);
        }

        public void NextVideo()
        {
            if (playlist.RowCount > 0)
            {
                foreach (DataGridViewRow row in playlist.Rows)
                {
                    if (row.Cells[1].Value.ToString() == currentID)
                    {
                        if (row.Index < playlist.RowCount - 1)
                        {
                            PlayVideo(row.Index + 1);
                        }
                        else
                        {
                            PlayVideo(0);
                        }

                        return;
                    }
                }

                if (playlist.RowCount < currentRow)
                {
                    PlayVideo(playlist.RowCount - 1);
                }
                else
                {
                    PlayVideo(currentRow - 1);
                }
            }
        }

        public void PreviousVideo()
        {
            if (playlist.RowCount > 0)
            {
                foreach (DataGridViewRow row in playlist.Rows)
                {
                    if (row.Cells[1].Value.ToString() == currentID)
                    {
                        if (row.Index > 0)
                        {
                            PlayVideo(row.Index - 1);
                        }
                        else
                        {
                            PlayVideo(playlist.RowCount - 1);
                        }

                        return;
                    }
                }

                if (currentRow == 1 || playlist.RowCount < currentRow)
                {
                    PlayVideo(playlist.RowCount - 1);
                }
                else
                {
                    PlayVideo(currentRow - 2);
                }
            }
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
            if (addButton.Visible)
            {
                addButton.Visible = false;
                searchBox.Size = new Size(playlist.Size.Width, searchBox.Size.Height);
            }

            if (searchBox.Text.Length == 0 || searchBox.ForeColor == SystemColors.ControlDark)
            {
                foreach (DataGridViewRow entry in playlist.Rows)
                {
                    entry.Visible = true;
                }

                return;
            }

            int row = -1;

            if (searchBox.Text.Length > 10)
            {
                row = EntryExists(searchBox.Text.Substring(searchBox.Text.Length - 11));

                if (row == -1 && VideoExists(searchBox.Text.Substring(searchBox.Text.Length - 11)))
                {
                    searchBox.Size = new Size(playlist.Size.Width - 104, searchBox.Size.Height);
                    addButton.Visible = true;

                    foreach (DataGridViewRow entry in playlist.Rows)
                    {
                        entry.Visible = true;
                    }

                    return;
                }
            }

            playlist.ClearSelection();

            foreach (DataGridViewRow entry in playlist.Rows)
            {
                if (entry.Index != row)
                {
                    entry.Visible = false;
                }
            }

            for (int c = 0; c < playlist.RowCount; c++)
            {
                for (int d = 0; d < playlist.ColumnCount - 2; d++)
                {
                    if (d != 1 && playlist.Rows[c].Cells[d].Value.ToString().ToLower().Contains(searchBox.Text.ToLower()))
                    {
                        playlist.Rows[c].Visible = true;

                        break;
                    }
                }
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            string ID = (searchBox.Text.Substring(searchBox.Text.Length - 11));

            string[] videoDetails = GetVideoDetails(ID);

            playlist.Rows.Add(playlist.RowCount + 1, ID, videoDetails[0], videoDetails[1], videoDetails[2], videoDetails[3]);

            ResetSearchBar();

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

                if (VideoExists(searchBox.Text.Substring(searchBox.Text.Length - 11)))
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
                ResetSearchBar();
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
            deletionIndex = playlist.SelectedRows[0].Index;

            for (int c = 1; c < playlist.SelectedRows.Count; c++)
            {
                if (playlist.SelectedRows[c].Index < deletionIndex)
                {
                    deletionIndex = playlist.SelectedRows[c].Index;
                }
            }

            foreach (DataGridViewRow row in playlist.SelectedRows)
            {
                if (row.Cells[1].Value.ToString() == currentID)
                {
                    videoNameLabel.Text = "[-] " + videoNameLabel.Text.Substring(videoNameLabel.Text.IndexOf("]") + 2);

                    return;
                }
            }
        }

        private void playlist_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            for (int c = deletionIndex; c < playlist.RowCount; c++)
            {
                playlist.Rows[c].Cells[0].Value = playlist.Rows[c].Index + 1;
            }

            if (player.Visible && videoNameLabel.Text.Substring(0, 3) != "[-]")
            {
                foreach (DataGridViewRow row in playlist.Rows)
                {
                    if (row.Cells[1].Value.ToString() == currentID)
                    {
                        currentRow = (int)row.Cells[0].Value;

                        videoNameLabel.Text = "[" + currentRow + "] " + videoNameLabel.Text.Substring(videoNameLabel.Text.IndexOf("]") + 2);

                        return;
                    }
                }
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

            SetFullView(false);

            ResetSearchBar();

            unsaved = false;
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            NextVideo();
        }

        private void previousButton_Click(object sender, EventArgs e)
        {
            PreviousVideo();
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
            foreach (DataGridViewRow row in playlist.Rows)
            {
                if (row.Cells[1].Value.ToString() == currentID)
                {
                    playlist.ClearSelection();

                    row.Selected = true;

                    break;
                }
            }
        }
    }

    public class JavascriptInterface
    {
        private MainForm form;

        public JavascriptInterface(MainForm form)
        {
            this.form = form;
        }

        public void NextVideo()
        {
            if (form.autoplay)
            {
                form.NextVideo();
            }
        }
    }
}
