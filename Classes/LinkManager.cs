using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Windows.Media.Animation;

namespace YoutubeDownloader.Classes
{
    static class LinkManager
    {
        public static string GetVideoID(string videoURL)
        {
            //https://www.youtube.com/watch?v=xOWH46e-p8M&hl=en

            string videoID = videoURL.Replace("https://www.youtube.com/watch?v=", "");

            if (videoID.Contains("&"))
            {
                videoID = videoID.Split("&")[0];
            }

            return videoID;
        }
    }
}
