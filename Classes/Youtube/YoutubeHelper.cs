using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using YoutubeDownloader.Classes.Youtube;

namespace YoutubeDownloader.Classes
{
    static class YoutubeHelper
    {
        //https://www.youtube.com/watch?v=wWQPnhG0xHU
        public static VideoInfo GetVideoData(string url)
        {
            var videoInfo = GetPlayerSources(url);

            videoInfo.jsFile = videoInfo.videoPage["assets"]["js"].ToString();

            videoInfo.PlayerResponse = JObject.Parse(videoInfo.rawVideoInfo["player_response"].ToString());        

            var StreamData = JObject.Parse(videoInfo.PlayerResponse["streamingData"].ToString());

            videoInfo.Formats = JArray.Parse(StreamData["formats"].ToString());
            videoInfo.AdaptiveFormats = JArray.Parse(StreamData["adaptiveFormats"].ToString());

            Debug.SaveFile(videoInfo.PlayerResponse.ToString(), "VideoInfoPlayerJson");
            Debug.SaveFile(videoInfo.Formats.ToString(), "Formats");
            Debug.SaveFile(videoInfo.AdaptiveFormats.ToString(), "AdFormats");

            Debug.SaveFile(JObject.Parse(videoInfo.rawVideoInfo["watch_next_response"].ToString()).ToString(), "WatchNextResponse");

            return videoInfo;
        }

        public static VideoInfo GetPlayerSources(string url)
        {
            VideoInfo videoInfo = new VideoInfo();

            var videoID = VideoLink.GetVideoID(url);
            string videoInfoLink =
                string.Format("https://www.youtube.com/get_video_info?video_id={0}&el=detailpage&hl=en", videoID);
          
            var rawVideoInfo = WebHelper.GetPageSouce(videoInfoLink);
            var videoPage = WebHelper.GetPageSouce(url);


            var dataRegex = new Regex(@"ytplayer\.config\s*=\s*(\{.+?\});", RegexOptions.Multiline);
            string extractedJson = dataRegex.Match(videoPage).Result("$1");


            videoInfo.rawVideoInfo = JsonHelper.ConvertToJson(rawVideoInfo);
            videoInfo.videoPage = JObject.Parse(extractedJson);


            Debug.SaveFile(videoInfo.rawVideoInfo.ToString(), "VideoInfoJson");
            Debug.SaveFile(videoInfo.videoPage.ToString(), "VideoPageJson");

            return videoInfo;
        }

        public static void DownloadFormat(JObject format)
        {

        }

    }
}
