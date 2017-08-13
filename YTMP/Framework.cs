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
        //The API key used to access YouTube's servers
        private const string API = "AIzaSyBMUx0bkglse9SvI2x69YAQZjARE3jsZG0";

        //Enables the program to download JSON files from YouTube's servers
        private WebClient WC = new WebClient();

        //De-serializes JSON files into dictionaries
        private JavaScriptSerializer serializer = new JavaScriptSerializer();

        

        private List<string> shuffleStack = new List<string>();

        public bool InvokeRequired { get; private set; }

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

        public void Export(DataGridView playlist, string fileName)
        {
            using (System.IO.StreamWriter SW = new System.IO.StreamWriter(fileName))
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
        }

        public void Import(DataGridView playlist, string fileName)
        {
            string ID;

            playlist.Rows.Clear();

            using (System.IO.StreamReader SR = new System.IO.StreamReader(fileName))
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
        }

        public void AddVideo(DataGridView playlist, string ID)
        {
            string[] details = GetVideoDetails(ID);

            playlist.Rows.Add(playlist.Rows.Count + 1, ID, details[0], details[1], details[2], details[3]);
        }

        public DataGridViewRow Advance(DataGridView playlist, DataGridViewRow current)
        {
            DataGridViewRow next = NextVideo(playlist, current.Cells[1].Value.ToString());

            if (next != null)
            {
                return next;
            }

            next = NextVideo(playlist, (int)current.Cells[0].Value - 1);

            if (next != null)
            {
                return next;
            }

            return null;
        }

        public DataGridViewRow NextVideo(DataGridView playlist, string currentID)
        {
            if (playlist.RowCount > 0)
            {
                foreach (DataGridViewRow row in playlist.Rows)
                {
                    if (row.Cells[1].Value.ToString() == currentID)
                    {
                        if (row.Index < playlist.RowCount - 1)
                        {
                            return playlist.Rows[row.Index + 1];
                        }
                        else
                        {
                            return playlist.Rows[0];
                        }
                    }
                }
            }

            return null;
        }

        public DataGridViewRow NextVideo(DataGridView playlist, int currentIndex)
        {
            if (playlist.RowCount > 0)
            {
                if (playlist.RowCount < currentIndex)
                {
                    return playlist.Rows[playlist.RowCount - 1];
                }
                else if (currentIndex == playlist.RowCount)
                {
                    return playlist.Rows[0];
                }
                else
                {
                    return playlist.Rows[currentIndex];
                }
            }

            return null;
        }

        public DataGridViewRow Retreat(DataGridView playlist, DataGridViewRow current)
        {
            DataGridViewRow previous = PreviousVideo(playlist, current.Cells[1].Value.ToString());

            if (previous != null)
            {
                return previous;
            }

            previous = PreviousVideo(playlist, (int)current.Cells[0].Value);

            if (previous != null)
            {
                return previous;
            }

            return null;
        }

        public DataGridViewRow PreviousVideo(DataGridView playlist, string currentID)
        {
            if (playlist.RowCount > 0)
            {
                foreach (DataGridViewRow row in playlist.Rows)
                {
                    if (row.Cells[1].Value.ToString() == currentID)
                    {
                        if (row.Index > 0)
                        {
                            return playlist.Rows[row.Index - 1];
                        }
                        else
                        {
                            return playlist.Rows[playlist.RowCount - 1];
                        }
                    }
                }
            }

            return null;
        }

        public DataGridViewRow PreviousVideo(DataGridView playlist, int currentIndex)
        {
            if (playlist.RowCount > 0)
            {
                if (playlist.RowCount < currentIndex || currentIndex == 0)
                {
                    return playlist.Rows[playlist.RowCount - 1];
                }
                else
                {
                    return playlist.Rows[currentIndex - 1];
                }
            }

            return null;
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
                    return true;
                }

                return false;
            }

            text = text.ToLower();

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

        public DataGridViewRow FindCurrentVideo(DataGridView playlist, DataGridViewRow current)
        {
            return FindVideo(playlist, current.Cells[1].Value.ToString());
        }
    }
}
