using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace YoutubeDownloader.Classes
{
    static class YoutubeVideo
    {
        //https://www.youtube.com/watch?v=wWQPnhG0xHU
        public static IVideoInfo GetVideoData(string url)
        { 
            var videoID = LinkHandler.GetVideoID(url);
            string videoInfoLink = 
                string.Format("https://www.youtube.com/get_video_info?video_id={0}&el=detailpage&hl=en", videoID);

            var rawVideoInfo = WebHelper.GetPageSouce(videoInfoLink);

            Debug.SaveFile(rawVideoInfo, "RawVideoInfo");

            IVideoInfo videoInfo = new IVideoInfo();

            videoInfo.rawVideoInfo = JsonHelper.ConvertToJson(rawVideoInfo);

            Debug.SaveFile(videoInfo.rawVideoInfo.ToString(), "VideoInfoJson");

            videoInfo.PlayerResponse = JObject.Parse(videoInfo.rawVideoInfo["player_response"].ToString());

            Debug.SaveFile(videoInfo.PlayerResponse.ToString(), "VideoInfoPlayerJson");

            var StreamData = JObject.Parse(videoInfo.PlayerResponse["streamingData"].ToString());

            videoInfo.Formats = JArray.Parse(StreamData["formats"].ToString());
            videoInfo.AdaptiveFormats = JArray.Parse(StreamData["adaptiveFormats"].ToString());

            Debug.SaveFile(videoInfo.Formats.ToString(), "Formats");//
            Debug.SaveFile(videoInfo.AdaptiveFormats.ToString(), "AdFormats");//

            return videoInfo;
        }

        public static void AnalyzeVideoData(string url)
        {

        }

    }
}
