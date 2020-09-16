using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;
using System.IO;
using System.Web;

namespace YoutubeDownloader.Class
{
    public static class JsonResponse
    {
        static JObject MainR;
        static JObject PlayerR;
        static JObject StreamData;

        public static void GetJsonResponse(string videoURL)
        {
            string HTMLSource;
            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                HTMLSource = client.DownloadString(videoURL);
            }

            Regex dataRegex = new Regex(@"ytplayer\.config\s*=\s*(\{.+?\});", RegexOptions.Multiline);

            string extractedJson = dataRegex.Match(HTMLSource).Result("$1");

            GetNeededData(JObject.Parse(extractedJson));

        }

        private static void GetNeededData(JObject response)
        {
            MainR = response;
            PlayerR = JObject.Parse(MainR["args"]["player_response"].ToString());
            StreamData = JObject.Parse(PlayerR["streamingData"].ToString());

            string[] whatthisdo = StreamData["adaptiveFormats"].ToString().Split(',');

            SaveFile(whatthisdo, "whatthisdo");

            var dictionary2 = new Dictionary<string, string>();
            var dictionary3 = new Dictionary<string, string>();

            foreach (string s in whatthisdo)
            {
                foreach (string vp in Regex.Split(s, "&"))
                {
                    string[] strings = Regex.Split(vp, "=");

                    dictionary2.Add(strings[0], strings.Length == 2 ? strings[1] : string.Empty);
                    dictionary3.Add(strings[0], strings.Length == 2 ? HttpUtility.UrlDecode(strings[1]) : string.Empty);
                }
            }
            SaveFile(dictionary2.ToString(), "dictionary2");
            SaveFile(dictionary3.ToString(), "dictionary3");
        }

        public static JObject GetResponse()
        {
            return MainR;
        }

        public static JObject GetPlayerResponse()
        {
            return PlayerR;
        }

        public static JObject GetStreamData()
        {
            return StreamData;
        }

        //For Debugging
        private static string savePath = "C:/Users/Admin/Downloads/YoutubeDownloader/";
        public static void SaveFile(string data, string filename, string extension = null)
        {
            File.WriteAllText(savePath + filename + extension, data);
        }
        public static void SaveFile(string[] data, string filename, string extension = null)
        {
            File.WriteAllLines(savePath + filename + extension, data);
        }
    }
}
    
