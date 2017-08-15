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
                        DJSON = serializer.Deserialize<Dictionary<String, object>>(WC.DownloadString("https://www.googleapis.com/youtube/v3/playlistItems?key=" + API + "&part=snippet,contentDetails&playlistId=" + ID));
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

        public object[] GetVideoDetails(string ID)
        {
            if (ID.Length < videoIDLength)
            {
                return null;
            }
            else
            {
                return new object[] { ID, ExtractVideoDetails(GetDJSON(ID, false)) };
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

        public void Export(Dictionary<string, object[]> playlist, Dictionary<int, string> listing, string fileName)
        {
            using (System.IO.StreamWriter SW = new System.IO.StreamWriter(fileName))
            {         
                for (int i = 0; i < listing.Count; i++)
                {
                    string ID = listing[i];

                    object[] entry = playlist[ID];

                    if (i < listing.Count - 1)
                    {
                        SW.WriteLine(ID + " - [" + (i + 1) + "] (" + entry[1] + ") " + entry[0]);
                    }
                    else
                    {
                        SW.Write(ID + " - [" + (i + 1) + "] (" + entry[1] + ") " + entry[0]);
                    }
                }
            }
        }

        public void Import(Dictionary<string, object[]> playlist, string fileName)
        {
            string ID;

            playlist.Clear();

            using (System.IO.StreamReader SR = new System.IO.StreamReader(fileName))
            {
                while ((ID = SR.ReadLine()) != null)
                {
                    if (ID.Length >= videoIDLength)
                    {
                        ID = ID.Substring(0, videoIDLength);

                        if (GetDJSON(ID, false) != null)
                        {
                            playlist[ID] = (object[])GetVideoDetails(ID)[1];
                        }
                    }
                }
            }
        }

        public string NextVideo(Dictionary<string, object[]> playlist, Dictionary<int, string> listing, ref int currentIndex)
        {
            if (playlist.Count > 0)
            {
                if (currentIndex > playlist.Count - 1 || currentIndex == playlist.Count - 1)
                {
                    currentIndex = 0;
                }
                else
                {
                    currentIndex += 1;
                }

                return listing[currentIndex];
            }

            return null;
        }

        public string PreviousVideo(Dictionary<string, object[]> playlist, Dictionary<int, string> listing, ref int currentIndex)
        {
            if (playlist.Count > 0)
            {
                if (currentIndex > playlist.Count - 1 || currentIndex == 0)
                {
                    currentIndex = playlist.Count - 1;
                }
                else
                {
                    currentIndex -= 1;
                }

                return listing[currentIndex];
            }

            return null;
        }
        
        public string[] Search(Dictionary<string, object[]> playlist, Dictionary<int, string> listing, string text)
        {
            if (playlist.Count == 0)
            {
                return new string[0];
            }

            if (text.Length == 0)
            {
                return playlist.Keys.ToArray();
            }

            if (text.Length >= videoIDLength && playlist[text.Substring(text.Length - videoIDLength)] != null)
            {
                return new string[] { text.Substring(text.Length - videoIDLength) };
            }

            text = text.ToLower();

            List<string> search = new List<string>();

            int parse;

            if (int.TryParse(text, out parse) && listing[parse] != null)
            {
                search.Add(listing[parse]);
            }

            foreach (string ID in playlist.Keys)
            {
                if (!search.Contains(ID))
                {
                    foreach (object item in playlist[ID])
                    {
                        if (item.ToString().ToLower().Contains(text))
                        {
                            search.Add(ID);

                            break;
                        }
                    }
                }
            }

            if (search.Count > 0)
            {
                return search.ToArray();
            }
            else
            {
                return new string[0];
            }
        }
    }
}
