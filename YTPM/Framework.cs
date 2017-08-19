using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace YTMP
{
    public class Framework
    {
        //The API key used to access YouTube's servers
        private const string API = "AIzaSyBMUx0bkglse9SvI2x69YAQZjARE3jsZG0";

        private const int videoIDLength = 11;

        private const int playlistIDLength = 34;

        //Enables the program to download JSON files from YouTube's servers
        private WebClient WC = new WebClient();

        //De-serializes JSON files into dictionaries
        private JavaScriptSerializer serializer = new JavaScriptSerializer();

        private List<string> shuffleStack = new List<string>();

        public bool InvokeRequired { get; private set; }

        public Dictionary<String, object> GetDJSON(string ID, bool playlist)
        {
            if ((!playlist && ID.Length < videoIDLength) || (playlist && ID.Length < playlistIDLength))
            {
                return null;
            }

            using (WC)
            {
                try
                {
                    Dictionary<String, object> DJSON;

                    if (!playlist)
                    {
                        if (!VideoExists(ID))
                        {
                            return null;
                        }

                        DJSON = serializer.Deserialize<Dictionary<String, object>>(WC.DownloadString("https://www.googleapis.com/youtube/v3/videos?key=" + API + "&part=snippet,contentDetails&id=" + ID));
                    }
                    else
                    {
                        DJSON = serializer.Deserialize<Dictionary<String, object>>(WC.DownloadString("https://www.googleapis.com/youtube/v3/playlistItems?key=" + API + "&part=contentDetails&playlistId=" + ID));
                    }

                    return DJSON;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        private object[] ExtractVideoDetails(Dictionary<String, object> DJSON)
        {
            if (DJSON == null)
            {
                return null;
            }

            object[] details = new object[4];

            dynamic partial;

            Dictionary<String, object> auxiliary;

            partial = (ArrayList)DJSON["items"];
            partial = partial[0];

            auxiliary = partial["contentDetails"];

            partial = (Dictionary<String, object>)partial["snippet"];

            details[0] = (string)partial["title"];
            details[1] = (string)partial["channelTitle"];

            string description = partial["description"].Replace("\n", "\r\n");
            details[2] = description;

            string timeString = (string)auxiliary["duration"];
            timeString = timeString.Substring(1).ToLower();

            int time = 0;

            if (timeString.Contains("d"))
            {
                time += int.Parse(timeString.Substring(0, timeString.IndexOf("d"))) * 86400;
            }

            timeString = timeString.Substring(timeString.IndexOf("t") + 1);

            if (timeString.Contains("h"))
            {
                time += int.Parse(timeString.Substring(0, timeString.IndexOf("h"))) * 3600;

                timeString = timeString.Substring(timeString.IndexOf("h") + 1);
            }

            if (timeString.Contains("m"))
            {
                time += int.Parse(timeString.Substring(0, timeString.IndexOf("m"))) * 60;

                timeString = timeString.Substring(timeString.IndexOf("m") + 1);
            }

            if (timeString.Contains("s"))
            {
                time += int.Parse(timeString.Substring(0, timeString.IndexOf("s")));
            }

            details[3] = time;

            return details;
        }
        
        public string[] CompilePlaylistVideos(string ID)
        {
            if (ID.Length < playlistIDLength)
            {
                return null;
            }

            Dictionary<string, object> DJSON = GetDJSON(ID, true);

            if (DJSON == null)
            {
                return null;
            }

            List<string> videos = new List<string>();

            do
            {
                string next = null;

                if (DJSON.ContainsKey("nextPageToken"))
                {
                    next = (string)DJSON["nextPageToken"];
                }

                ArrayList partial = (ArrayList)DJSON["items"];

                for (int i = 0; i < partial.Count; i++)
                {
                    dynamic item = partial[i];

                    item = item["contentDetails"];

                    videos.Add(item["videoId"]);
                }

                if (next != null)
                {
                    DJSON = GetDJSON(ID + "&pageToken=" + next, true);
                }
                else
                {
                    DJSON = null;
                }
            } while (DJSON != null);

            if (videos.Count > 0) { return videos.ToArray(); } else { return null; }
        }

        public object[] GetVideoDetails(string ID)
        {
            if (ID.Length < videoIDLength)
            {
                return null;
            }
            else
            {
                Dictionary<String, object> DJSON = GetDJSON(ID, false);

                if (DJSON == null) { return null; } else { return new object[] { ID, ExtractVideoDetails(DJSON) }; }
            }
        }

        public bool VideoExists(string ID)
        {
            if (ID.Length < videoIDLength)
            {
                return false;
            }

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

        public bool YouTubeAvailable(bool connect)
        {
            if (!NetworkInterface.GetIsNetworkAvailable()) { return false; }

            if (!connect) { return true; }

            try
            {
                using (HttpWebResponse HTTP = (HttpWebResponse)WebRequest.Create("https://www.youtube.com/").GetResponse())
                {
                    return true;
                }
            }
            catch (WebException WE) { return false; }
        }

        public string[] Import(Dictionary<string, object[]> playlist, string fileName, bool full)
        {
            string line, ID;

            List<string> results = new List<string>();

            playlist.Clear();

            try
            {
                using (System.IO.StreamReader SR = new System.IO.StreamReader(fileName))
                {
                    while ((line = SR.ReadLine()) != null)
                    {
                        if (line.Length >= videoIDLength)
                        {
                            ID = line.Substring(0, videoIDLength);

                            if (full)
                            {
                                object[] details = GetVideoDetails(ID);

                                if (details != null)
                                {
                                    details = (object[])details[1];

                                    playlist[ID] = details;

                                    results.Add(ID);
                                }
                            }
                            else
                            {
                                bool valid = true;

                                for (int i = 0; i < ID.Length; i++)
                                {
                                    if (!char.IsLetterOrDigit(ID[i]) && ID[i] != char.Parse("_"))
                                    {
                                        valid = false;
                                    }
                                }

                                if (valid)
                                {
                                    playlist[ID] = new object[] { line, string.Empty, string.Empty, null };

                                    results.Add(ID);
                                }
                            }
                        }
                    }
                }
            } catch (Exception E) { playlist.Clear(); return null; }

            if (results.Count > 0) { return results.ToArray(); } else { return null; }
        }
    }
}
