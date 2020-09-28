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
using YoutubeDownloader.Classes;
using System.Threading.Tasks;
using System.Linq;
using YoutubeDownloader.Classes.Youtube;

namespace YoutubeDownloader.Class
{
    public static class JsonResponse
    {
        static JObject MainR;
        static JObject PlayerR;
        static JObject StreamData;

        public static void GetJsonResponse(string videoURL)
        {

            string videoID = VideoLink.GetVideoID(videoURL);

            string HTMLDetails;
            //string videoDetails = string.Format("https://www.youtube.com/get_video_info?video_id={0}&el=detailpage", videoID);
            string videoDetails = videoURL;
            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;

                HTMLDetails = client.DownloadString(videoDetails);
            }
            Debug.SaveFile(HTMLDetails, "YoutubeInfo");
            /*
            var dictionary1 = new Dictionary<string, string>();

            foreach (string vp in Regex.Split(HTMLDetails, "&"))
            {
                string[] strings = Regex.Split(vp, "=");

                dictionary1.Add(strings[0], strings.Length == 2 ? HttpUtility.UrlDecode(strings[1]) : string.Empty);
            }

            JObject dic1 = JObject.Parse(JsonConvert.SerializeObject(dictionary1).ToString());

            SaveFile(dic1.ToString(), "dictionary1");

             GetNeededData(dic1);
            */


            Regex dataRegex = new Regex(@"ytplayer\.config\s*=\s*(\{.+?\});", RegexOptions.Multiline);
            string extractedJson = dataRegex.Match(HTMLDetails).Result("$1");
            Debug.SaveFile(extractedJson, "EmbedPageJSON");

            GetNeededData(JObject.Parse(extractedJson));
        }


        private const string RateBypassFlag = "ratebypass";
        private const string SignatureQuery = "signature";
        private static void GetNeededData(JObject response)
        {
            MainR = response;
            var assetsjs = MainR["assets"]["js"].ToString();
            PlayerR = JObject.Parse(MainR["args"]["player_response"].ToString());
            StreamData = JObject.Parse(PlayerR["streamingData"].ToString());

            Debug.SaveFile(MainR.ToString(), "YoutubeResponse");
            Debug.SaveFile(PlayerR.ToString(), "YoutubePlayerResponse");
            Debug.SaveFile(StreamData.ToString(), "VideoStreamData");

            JArray AdFormats = JArray.Parse(StreamData["formats"].ToString());

            Debug.SaveFile(AdFormats.ToString(), "VideoAdaptiveFormats");

            
            var dictionary3 = new Dictionary<string, string>();


            foreach (string vp in Regex.Split(AdFormats[0]["signatureCipher"].ToString(), "&"))
            {
                string[] strings = Regex.Split(vp, "=");

                dictionary3.Add(strings[0], strings.Length == 2 ? HttpUtility.UrlDecode(strings[1]) : string.Empty);
            }

            JObject dic3 = JObject.Parse(JsonConvert.SerializeObject(dictionary3).ToString());

            Debug.SaveFile(dic3.ToString(), "dictionary3");

            findd(dic3, assetsjs);

            /*
            string url;

                url = dictionary3["url"];

                SaveFile(url, "urlsmall");
            

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

            SaveFile(dic4.ToString(), "dictionary4_4");

            IDictionary<string, string> parameters = dictionary4;
            if (!parameters.ContainsKey(RateBypassFlag))
            {
                url += string.Format("&{0}={1}", RateBypassFlag, "yes");
            }

            SaveFile(url, "URLDECP");
            /*
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



        private static void findd(JObject idk, string assetsjs)
        {
            string js;
            string jsUrl =
                string.Format("https://www.youtube.com{0}", assetsjs);
            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                js = client.DownloadString(jsUrl);
            }
            Debug.SaveFile(js, "JSSource");

            
            string? TryGetDeciphererFuncBody()
            {
                var funcName = Regex.Match(js, @"(\w+)=function\(\w+\){(\w+)=\2\.split\(\x22{2}\);.*?return\s+\2\.join\(\x22{2}\)}")
                    .Groups[0]
                    .Value;

                return funcName;
            }

            Debug.SaveFile(TryGetDeciphererFuncBody().ToString(), "FunctionName");

            string? TryGetDeciphererDefinitionBody(string body)
            {
                var objName = Regex.Match(body, "([\\$_\\w]+).\\w+\\(\\w+,\\d+\\);")
                    .Groups[1]
                    .Value;

                var escapedObjName = Regex.Escape(objName);

                return Regex.Match(js, $@"var\s+{escapedObjName}=\{{(\w+:function\(\w+(,\w+)?\)\{{(.*?)\}}),?\}};", RegexOptions.Singleline)
                    .Groups[0]
                    .Value;
  
            }

            

            Debug.SaveFile(TryGetDeciphererDefinitionBody(TryGetDeciphererFuncBody()).ToString(), "FunctionBody");


        }

        public static string DecipherWithVersion(string cipher, string cipherVersion)
        {
        
            //string jsUrl = string.Format("https://www.youtube.com{0}", cipherVersion);
            string jsUrl = string.Format("https://www.youtube.com/yts/jsbin/player-en_US-vfl8LqiZp/base.js", cipherVersion);
            string js;
            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                js = client.DownloadString(jsUrl);
            }

            Debug.SaveFile(js, "JSSource");

            //Find "C" in this: var A = B.sig||C (B.s)
            string functNamePattern = @"\""signature"",\s?([a-zA-Z0-9\$]+)\("; //Regex Formed To Find Word or DollarSign

            var funcName = Regex.Match(js, functNamePattern);

            Debug.SaveFile(funcName.ToString(), "Function name");

            return funcName.ToString();
            /*
           // if (funcName.ToString().Contains("$"))
           // {
           //     funcName = "\\" + funcName; //Due To Dollar Sign Introduction, Need To Escape
           // }

            string funcPattern = @"(?!h\.)" + @funcName + @"=function\(\w+\)\{.*?\}"; //Escape funcName string
            var funcBody = Regex.Match(js, funcPattern, RegexOptions.Singleline).Value; //Entire sig function

            return funcBody;
            
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


    }
}
    
