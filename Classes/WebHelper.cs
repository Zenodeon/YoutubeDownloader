﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using YoutubeDownloader.Classes.Youtube;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace YoutubeDownloader.Classes
{
    static class WebHelper
    {
        public static string GetPageSouce(string url)
        {
            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;

                return client.DownloadString(url);
            }
        }

        public static async Task<BitmapImage> GetThumbnailAsync(string videoURL, IProgress<DownloadProgressData> progress)
        {
            string videoID = VideoLink.GetVideoID(videoURL);
            string thumbURL = string.Format("https://i.ytimg.com/vi/{0}/maxresdefault.jpg", videoID);
            string fileName = string.Format("thumb-{0}.jpg", videoID);

            await Task.Run(() => Download.DownloadContent(thumbURL, fileName, Debug.savePath, progress));

            var path = new Uri(Debug.savePath + fileName);
            return new BitmapImage(path);
        }

        public static async Task DownloadVideoAsync(JObject format, VideoInfo videoInfo, IProgress<DownloadProgressData> progress)
        {

            string signature = format["signatureCipher"].ToString();

            JObject signatureCipher = JsonHelper.ConvertToJson(signature);

            //Decrypter.Decrypt(signatureCipher);







            //string fileName = string.Format("video-{0}.mp4", videoID);

            //await Task.Run(() => Download.DownloadContent(videoLink, fileName, Debug.savePath, progress));
        }
    }
}
