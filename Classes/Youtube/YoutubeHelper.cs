using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
            var videoID = VideoLink.GetVideoID(url);
            string videoInfoLink = 
                string.Format("https://www.youtube.com/get_video_info?video_id={0}&el=detailpage&hl=en", videoID);

            var rawVideoInfo = WebHelper.GetPageSouce(videoInfoLink);

            Debug.SaveFile(rawVideoInfo, "RawVideoInfo");

            VideoInfo videoInfo = new VideoInfo();

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

        public static void DownloadFormat(JObject format)
        {

        }

    }
}
