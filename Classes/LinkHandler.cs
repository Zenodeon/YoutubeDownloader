using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace YoutubeDownloader.Classes
{
    static class LinkHandler
    {
        public static string GetVideoID(string videoURL)
        {
            //https://www.youtube.com/watch?v=xOWH46e-p8M&hl=en
            //https://www.youtube.com/watch?v=HZrSpgWBAJU

            string videoID = videoURL.Replace("https://www.youtube.com/watch?v=", "");

            if (videoID.Contains("&"))
            {
                videoID = videoID.Split("&")[0];
            }

            return videoID;
        }

        public static bool IsValidLink(string url)
        {
            var videoID = GetVideoID(url);

            if(videoID.Length == 11)
            {
                return true;
            }

            return false;
        }
        

    }
}
