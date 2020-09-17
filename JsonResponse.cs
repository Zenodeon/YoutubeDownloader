using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;
using System.IO;
using System.Web;
using Newtonsoft.Json;
using System.Net.Http;

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

            string videoId = "xOWH46e-p8M";
            string sts = null;
            var eurl = WebUtility.HtmlEncode($"https://youtube.googleapis.com/v/{videoId}");

            var url = $"https://youtube.com/get_video_info?video_id={videoId}&el=embedded&eurl={eurl}&hl=en&sts={sts}";
            //var raw =  HttpClient.GetStringAsync(url);

            //var result = Parse(raw);

            //GetNeededData(JObject.Parse(extractedJson));

        }



        private const string SignatureQuery = "signature";
        private static void GetNeededData(JObject response)
        {
            MainR = response;
            PlayerR = JObject.Parse(MainR["args"]["player_response"].ToString());
            StreamData = JObject.Parse(PlayerR["streamingData"].ToString());

            SaveFile(StreamData.ToString(), "VideoStreamData");

            //string[] whatthisdo = StreamData["adaptiveFormats"].ToString().Split(',');

            JArray AdFormats = JArray.Parse(StreamData["adaptiveFormats"].ToString());

            SaveFile(AdFormats[1].ToString(), "VideoAdaptiveFormats");


            var dictionary2 = new Dictionary<string, string>();
            var dictionary3 = new Dictionary<string, string>();


            foreach (string vp in Regex.Split(AdFormats[1]["signatureCipher"].ToString(), "&"))
            {
                string[] strings = Regex.Split(vp, "=");

                dictionary2.Add(strings[0], strings.Length == 2 ? strings[1] : string.Empty);
                dictionary3.Add(strings[0], strings.Length == 2 ? HttpUtility.UrlDecode(strings[1]) : string.Empty);
            }
            JObject dic2 = JObject.Parse(JsonConvert.SerializeObject(dictionary2).ToString());
            JObject dic3 = JObject.Parse(JsonConvert.SerializeObject(dictionary3).ToString());

            SaveFile(dic2.ToString(), "dictionary2");
            SaveFile(dic3.ToString(), "dictionary3");

            string url;

            if (dictionary3.ContainsKey("s") || dictionary3.ContainsKey("sig"))
            {
                //requiresDecryption = dictionary3.ContainsKey("s");
                string signature = dictionary3.ContainsKey("s") ? dictionary3["s"] : dictionary3["sig"];

                url = string.Format("{0}&{1}={2}", dictionary3["url"], SignatureQuery, signature);

                string fallbackHost = dictionary3.ContainsKey("fallback_host") ? "&fallback_host=" + dictionary3["fallback_host"] : String.Empty;

                url += fallbackHost;

                SaveFile(url, "urlbig");
                SaveFile(fallbackHost, "fallbackHost");
            }

            else
            {
                url = dictionary3["url"];

                SaveFile(url, "urlsmall");
            }
            SaveFile(url, "URLBEFORE");
            url = HttpUtility.UrlDecode(url);
            SaveFile(url, "URLMID");
            url = HttpUtility.UrlDecode(url);
            SaveFile(url, "URLAFTER");
            Uri uri = new Uri(url);

            SaveFile(uri.ToString(), "URI");

            var request = (HttpWebRequest)WebRequest.Create(uri);


           
            using (WebResponse Wresponse = request.GetResponse())
            {
                using (Stream source = Wresponse.GetResponseStream())
                {
                    using (FileStream target = File.Open(savePath, FileMode.Create, FileAccess.Write))
                    {
                        var buffer = new byte[1024];
                        bool cancel = false;
                        int bytes;
                        int copiedBytes = 0;

                        while (!cancel && (bytes = source.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            target.Write(buffer, 0, bytes);

                            copiedBytes += bytes;

                            //(copiedBytes * 1.0 / Wresponse.ContentLength) * 100);

                        }
                    }
                }
            }
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
    
