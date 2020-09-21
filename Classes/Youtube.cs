using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace YoutubeDownloader.Classes
{
    static class Youtube
    {
        public static Uri GetVideo(string url)
        {
            string VideoPageSource = HttpHandler.GetPageSouce(url);

            Debug.SaveFile(VideoPageSource, "VideoPageSource");

            JObject PlayerResponse = GetPlayerResponse(GetVideoPageJson(VideoPageSource));

            Debug.SaveFile(GetVideoPageJson(VideoPageSource).ToString(), "VideoPageJSON");
            Debug.SaveFile(PlayerResponse.ToString(), "PlayerResponseJson");

            var StreamData = JObject.Parse(PlayerResponse["streamingData"].ToString());

            JArray AdFormats = JArray.Parse(StreamData["formats"].ToString());

            Debug.SaveFile(AdFormats.ToString(), "AdFormats");

            return new Uri(AdFormats[1]["url"].ToString());
        }

        private static JObject GetVideoPageJson(string pageSource)
        {
            Regex dataRegex = new Regex(@"ytplayer\.config\s*=\s*(\{.+?\});", RegexOptions.Multiline);
            return JObject.Parse(dataRegex.Match(pageSource).Result("$1"));           
        }

        private static JObject GetPlayerResponse(JObject pageJson)
        {
            return JObject.Parse(pageJson["args"]["player_response"].ToString());
        }
    }
}
