using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace YoutubeDownloader.Classes
{
    static class Download
    {
        private static string savePath = "C:/Users/Admin/Downloads/YoutubeDownloader/video.mp4";
        public static void Video(String url)
        {
            var request = (HttpWebRequest)WebRequest.Create(Youtube.GetVideo(url));

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
        }
    }
}
