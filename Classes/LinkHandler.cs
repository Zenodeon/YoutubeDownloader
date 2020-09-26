﻿using System;
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
        
        public static async Task<BitmapImage> GetThumbnailAsync(string videoURL, IProgress<IProgressData> progress)
        {
            string videoID = GetVideoID(videoURL);
            string thumbURL = string.Format("https://i.ytimg.com/vi/{0}/maxresdefault.jpg", videoID);
            string fileName = string.Format("thumb-{0}.jpg", videoID);

            await Task.Run(() => Download.DownloadContent(thumbURL, fileName, Debug.savePath, progress));

            var path = new Uri(Debug.savePath + fileName);
            return new BitmapImage(path);
        }

        public static async Task DownloadVideo(string videoURL, IProgress<IProgressData> progress)
        {
            string videoID = GetVideoID(videoURL);
            string videoLink = Youtube.GetVideo(videoURL).ToString();
            string fileName = string.Format("video-{0}.mp4", videoID);

            await Task.Run(() => Download.DownloadContent(videoLink, fileName, Debug.savePath, progress));
        }
    }
}
