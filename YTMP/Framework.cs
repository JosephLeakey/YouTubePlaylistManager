using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace YTMP
{
    public class Framework
    {
        private MainForm form;

        //The API key used to access YouTube's servers
        private const string API = "AIzaSyBMUx0bkglse9SvI2x69YAQZjARE3jsZG0";

        //Enables the program to download JSON files from YouTube's servers
        private WebClient WC = new WebClient();

        //De-serializes JSON files into dictionaries
        private JavaScriptSerializer serializer = new JavaScriptSerializer();

        //Tracks the indexes of the playlist entries that users may choose to delete
        private int deletionIndex;

        private int currentRow;

        private string currentID;

        public bool autoplay = true;

        public bool shuffle = false;

        private List<string> shuffleStack = new List<string>();

        public bool InvokeRequired { get; private set; }

        public Framework(MainForm form)
        {
            this.form = form;
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

        private object[] GetPlaylistEntryDetails(DataGridView playlist, int index)
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

        private int EntryExists(DataGridView playlist, string ID)
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

        public bool Export(DataGridView playlist)
        {
            if (form.saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (System.IO.StreamWriter SW = new System.IO.StreamWriter(form.saveFileDialog.FileName))
                {
                    int rowCount = playlist.RowCount;

                    for (int i = 0; i < rowCount; i++)
                    {
                        if (i < rowCount - 1)
                        {
                            SW.WriteLine(playlist.Rows[i].Cells[1].Value + " - [" + playlist.Rows[i].Cells[3].Value + "] " + playlist.Rows[i].Cells[2].Value);
                        }
                        else
                        {
                            SW.Write(playlist.Rows[i].Cells[1].Value + " - [" + playlist.Rows[i].Cells[3].Value + "] " + playlist.Rows[i].Cells[2].Value);
                        }
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Import(DataGridView playlist, bool export)
        {
            if (export)
            {
                switch (MessageBox.Show("Your current playlist hasn't been saved.\nWould you like to save it before you open another one?", "Unsaved Playlist", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning))
                {
                    case (DialogResult.Yes):
                        Export(playlist);
                        break;
                    case (DialogResult.Cancel):
                        return false;
                }
            }

            if (form.loadFileDialog.ShowDialog() == DialogResult.OK)
            {
                string ID;

                playlist.Rows.Clear();

                using (System.IO.StreamReader SR = new System.IO.StreamReader(form.loadFileDialog.FileName))
                {
                    while ((ID = SR.ReadLine()) != null)
                    {
                        if (ID.Length > 10)
                        {
                            ID = ID.Substring(0, 11);

                            if (VideoExists(ID))
                            {
                                AddVideo(playlist, ID);
                            }
                        }
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddVideo(DataGridView playlist, string ID)
        {
            string[] details = GetVideoDetails(ID);

            playlist.Rows.Add(playlist.Rows.Count + 1, ID, details[0], details[1], details[2], details[3]);
        }

        public void PlayVideo(DataGridViewRow entry)
        {
            currentRow = (int)entry.Cells[0].Value;
            currentID = (string)entry.Cells[1].Value;

            form.ExecuteJavaScript(@"player.loadVideoById(""" + currentID + @""")");

            if (InvokeRequired)
            {
                form.BeginInvoke((MethodInvoker)delegate () {
                    form.UpdateVideoDetails(entry);
                });
            }
            else
            {
                form.UpdateVideoDetails(entry);
            }

            form.UpdateUIElements();

            form.SetFullView(true);
        }

        public void Advance(DataGridView playlist)
        {
            if (!NextVideo(playlist, currentID))
            {
                NextVideo(playlist, currentRow);
            }
        }

        public bool NextVideo(DataGridView playlist, string currentID)
        {
            if (playlist.RowCount > 0)
            {
                foreach (DataGridViewRow row in playlist.Rows)
                {
                    if (row.Cells[1].Value.ToString() == currentID)
                    {
                        if (row.Index < playlist.RowCount - 1)
                        {
                            PlayVideo(playlist.Rows[row.Index + 1]);
                        }
                        else
                        {
                            PlayVideo(playlist.Rows[0]);
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        public bool NextVideo(DataGridView playlist, int currentNumber)
        {
            if (playlist.RowCount > 0)
            {
                if (playlist.RowCount < currentNumber)
                {
                    PlayVideo(playlist.Rows[playlist.RowCount - 1]);
                }
                else if (currentNumber == playlist.RowCount)
                {
                    PlayVideo(playlist.Rows[0]);
                }
                else
                {
                    PlayVideo(playlist.Rows[currentNumber]);
                }

                return true;
            }

            return false;
        }

        public void Retreat(DataGridView playlist)
        {
            if (!PreviousVideo(playlist, currentID))
            {
                PreviousVideo(playlist, currentRow);
            }
        }

        public bool PreviousVideo(DataGridView playlist, string currentID)
        {
            if (playlist.RowCount > 0)
            {
                foreach (DataGridViewRow row in playlist.Rows)
                {
                    if (row.Cells[1].Value.ToString() == currentID)
                    {
                        if (row.Index > 0)
                        {
                            PlayVideo(playlist.Rows[row.Index - 1]);
                        }
                        else
                        {
                            PlayVideo(playlist.Rows[playlist.RowCount - 1]);
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        public bool PreviousVideo(DataGridView playlist, int currentNumber)
        {
            if (playlist.RowCount > 0)
            {
                if (playlist.RowCount < currentNumber || currentNumber == 0)
                {
                    PlayVideo(playlist.Rows[playlist.RowCount - 1]);
                }
                else
                {
                    PlayVideo(playlist.Rows[currentNumber - 2]);
                }

                return true;
            }

            return false;
        }

        public bool Search(DataGridView playlist, string text)
        {
            foreach (DataGridViewRow entry in playlist.Rows)
            {
                entry.Visible = false;
            }

            int row = -1;

            if (text.Length > 10 && VideoExists(text.Substring(text.Length - 11)))
            {
                row = EntryExists(playlist, text.Substring(text.Length - 11));

                foreach (DataGridViewRow entry in playlist.Rows)
                {
                    if (entry.Index == row || row == -1)
                    {
                        entry.Visible = true;
                    }
                }

                if (row != -1)
                {
                    return false;
                }
            }

            for (int c = 0; c < playlist.RowCount; c++)
            {
                for (int d = 0; d < playlist.Rows[0].Cells.Count - 2; d++)
                {
                    if (d != 1 && playlist.Rows[c].Cells[d].Value.ToString().ToLower().Contains(text))
                    {
                        playlist.Rows[c].Visible = true;

                        break;
                    }
                }
            }

            return true;
        }

        public void DeletionPreparation(DataGridViewSelectedRowCollection rows)
        {
            deletionIndex = rows[0].Index;

            for (int c = 1; c < rows.Count; c++)
            {
                if (rows[c].Index < deletionIndex)
                {
                    deletionIndex = rows[c].Index;
                }
            }

            foreach (DataGridViewRow row in rows)
            {
                if (row.Cells[1].Value.ToString() == currentID)
                {
                    form.UpdateVideoNameTag(0);

                    return;
                }
            }
        }

        public void DeletionCompletion(DataGridView playlist)
        {
            for (int c = deletionIndex; c < playlist.RowCount; c++)
            {
                playlist.Rows[c].Cells[0].Value = c + 1;
            }

            if (form.FullViewActive() && form.VideoInPlaylist())
            {
                foreach (DataGridViewRow row in playlist.Rows)
                {
                    if (row.Cells[1].Value.ToString() == currentID)
                    {
                        form.UpdateVideoNameTag((int)row.Cells[0].Value);

                        return;
                    }
                }
            }
        }

        public void NewPlaylist(DataGridView playlist)
        {
            if (playlist != null)
            {
                switch (MessageBox.Show("Your current playlist hasn't been saved.\nWould you like to save it before you make a new one?", "Unsaved Playlist", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning))
                {
                    case (DialogResult.Yes):
                        Export(playlist);
                        break;
                    case (DialogResult.Cancel):
                        return;
                }
            }

            form.ClearPlaylist();

            form.SetFullView(false);

            form.SetSearchBar(0);
        }

        public bool ToggleAutoplay()
        {
            autoplay = !autoplay;

            return autoplay;
        }

        public DataGridViewRow FindVideo(DataGridView playlist, string ID)
        {
            foreach (DataGridViewRow row in playlist.Rows)
            {
                if (row.Cells[1].Value.ToString() == ID)
                {
                    return row;
                }
            }

            return null;
        }

        public DataGridViewRow FindCurrentVideo(DataGridView playlist)
        {
            return FindVideo(playlist, currentID);
        }
    }
}
