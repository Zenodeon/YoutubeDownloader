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

namespace YoutubeDownloader.Classes
{
    static class LinkHandler
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

        public static bool IsValidLink(string url)
        {
            var videoID = GetVideoID(url);

            if(videoID.Length == 11)
            {
                return true;
            }

            return false;
        }

        public static BitmapImage GetThumbnail(string videoURL)
        {
            string videoID = GetVideoID(videoURL);
            string thumbURL = string.Format("https://i.ytimg.com/vi/{0}/maxresdefault.jpg", videoID);
            string fileName = string.Format("thumb-{0}.jpg", videoID);

            var request = (HttpWebRequest)WebRequest.Create(thumbURL);

            using (WebResponse Wresponse = request.GetResponse())
            {
                using (Stream source = Wresponse.GetResponseStream())
                {
                    using (FileStream target = File.Open(Debug.savePath + fileName, FileMode.Create, FileAccess.Write))
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
            var path = new Uri(Debug.savePath + fileName);
            return new BitmapImage(path);
        }
    }
}
