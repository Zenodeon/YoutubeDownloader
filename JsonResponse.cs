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

            GetNeededData(JObject.Parse(extractedJson));

        }


        private const string RateBypassFlag = "ratebypass";
        private const string SignatureQuery = "signature";
        private static void GetNeededData(JObject response)
        {
            MainR = response;
            PlayerR = JObject.Parse(MainR["args"]["player_response"].ToString());
            StreamData = JObject.Parse(PlayerR["streamingData"].ToString());

            SaveFile(MainR.ToString(), "YoutubeResponse");
            SaveFile(PlayerR.ToString(), "YoutubePlayerResponse");
            SaveFile(StreamData.ToString(), "VideoStreamData");

            JArray AdFormats = JArray.Parse(StreamData["adaptiveFormats"].ToString());

            SaveFile(AdFormats[1].ToString(), "VideoAdaptiveFormats");


            var dictionary3 = new Dictionary<string, string>();


            foreach (string vp in Regex.Split(AdFormats[1]["signatureCipher"].ToString(), "&"))
            {
                string[] strings = Regex.Split(vp, "=");

                dictionary3.Add(strings[0], strings.Length == 2 ? HttpUtility.UrlDecode(strings[1]) : string.Empty);
            }

            JObject dic3 = JObject.Parse(JsonConvert.SerializeObject(dictionary3).ToString());

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

            url = HttpUtility.UrlDecode(url);
            url = HttpUtility.UrlDecode(url);

            SaveFile(url, "URL");



            var surl = url.Substring(url.IndexOf('?') + 1);
            SaveFile(surl, "SURL");

            var dictionary4 = new Dictionary<string, string>();

            foreach (string su in Regex.Split(surl, "&"))
            {
                string[] strings = Regex.Split(su, "=");

                dictionary4.Add(strings[0], strings.Length == 2 ? HttpUtility.UrlDecode(strings[1]) : string.Empty);
            }

            JObject dic4 = JObject.Parse(JsonConvert.SerializeObject(dictionary4).ToString());

            SaveFile(dic4.ToString(), "dictionary4");

            IDictionary<string, string> parameters = dictionary4;
            if (!parameters.ContainsKey(RateBypassFlag))
            {
                url += string.Format("&{0}={1}", RateBypassFlag, "yes");
            }

            SaveFile(url, "URLDECP");

            string html5version = MainR["assets"]["js"].ToString();

            SaveFile(html5version, "HTMLVERSION");

            string Debbuging = DecipherWithVersion(AdFormats[1]["signatureCipher"].ToString(), html5version);

            SaveFile(Debbuging, "Debbugged Values");

            /*
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
            */
        }
        public static string DecipherWithVersion(string cipher, string cipherVersion)
        {
            string jsUrl = string.Format("http://www.youtube.com{0}", cipherVersion);
            string js;
            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                js = client.DownloadString(jsUrl);
            }

            

            //Find "C" in this: var A = B.sig||C (B.s)
            string functNamePattern = @"\""signature"",\s?([a-zA-Z0-9\$]+)\("; //Regex Formed To Find Word or DollarSign

            var funcName = Regex.Match(js, functNamePattern).Groups[1].Value;

            return funcName.ToString();
            /*
            if (funcName.Contains("$"))
            {
                funcName = "\\" + funcName; //Due To Dollar Sign Introduction, Need To Escape
            }

            string funcPattern = @"(?!h\.)" + @funcName + @"=function\(\w+\)\{.*?\}"; //Escape funcName string
            var funcBody = Regex.Match(js, funcPattern, RegexOptions.Singleline).Value; //Entire sig function
            var lines = funcBody.Split(';'); //Each line in sig function

            string idReverse = "", idSlice = "", idCharSwap = ""; //Hold name for each cipher method
            string functionIdentifier = "";
            string operations = "";

            foreach (var line in lines.Skip(1).Take(lines.Length - 2)) //Matches the funcBody with each cipher method. Only runs till all three are defined.
            {
                if (!string.IsNullOrEmpty(idReverse) && !string.IsNullOrEmpty(idSlice) &&
                    !string.IsNullOrEmpty(idCharSwap))
                {
                    break; //Break loop if all three cipher methods are defined
                }

                functionIdentifier = GetFunctionFromLine(line);
                string reReverse = string.Format(@"{0}:\bfunction\b\(\w+\)", functionIdentifier); //Regex for reverse (one parameter)
                string reSlice = string.Format(@"{0}:\bfunction\b\([a],b\).(\breturn\b)?.?\w+\.", functionIdentifier); //Regex for slice (return or not)
                string reSwap = string.Format(@"{0}:\bfunction\b\(\w+\,\w\).\bvar\b.\bc=a\b", functionIdentifier); //Regex for the char swap.

                if (Regex.Match(js, reReverse).Success)
                {
                    idReverse = functionIdentifier; //If def matched the regex for reverse then the current function is a defined as the reverse
                }

                if (Regex.Match(js, reSlice).Success)
                {
                    idSlice = functionIdentifier; //If def matched the regex for slice then the current function is defined as the slice.
                }

                if (Regex.Match(js, reSwap).Success)
                {
                    idCharSwap = functionIdentifier; //If def matched the regex for charSwap then the current function is defined as swap.
                }
            }

            foreach (var line in lines.Skip(1).Take(lines.Length - 2))
            {
                Match m;
                functionIdentifier = GetFunctionFromLine(line);

                if ((m = Regex.Match(line, @"\(\w+,(?<index>\d+)\)")).Success && functionIdentifier == idCharSwap)
                {
                    operations += "w" + m.Groups["index"].Value + " "; //operation is a swap (w)
                }

                if ((m = Regex.Match(line, @"\(\w+,(?<index>\d+)\)")).Success && functionIdentifier == idSlice)
                {
                    operations += "s" + m.Groups["index"].Value + " "; //operation is a slice
                }

                if (functionIdentifier == idReverse) //No regex required for reverse (reverse method has no parameters)
                {
                    operations += "r "; //operation is a reverse
                }
            }

            operations = operations.Trim();

            return DecipherWithOperations(cipher, operations);
            */

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
    
