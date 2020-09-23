using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace YoutubeDownloader.Classes
{
    static class WebHandler
    {
        public static string GetPageSouce(string url)
        {
            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;

                return client.DownloadString(url);
            }
        }

        public static bool cancelDownloads = false;

        public static Double Progress;
        public static void DownloadContent(String url, string fileName, string fileSavePath)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);

            using (WebResponse Wresponse = request.GetResponse())
            {
                using (Stream source = Wresponse.GetResponseStream())
                {
                    using (FileStream target = File.Open(fileSavePath + fileName, FileMode.Create, FileAccess.Write))
                    {
                        var buffer = new byte[1024];

                        int bytes;
                        int copiedBytes = 0;

                        while (!cancelDownloads && (bytes = source.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            target.Write(buffer, 0, bytes);

                            copiedBytes += bytes;

                            Progress = ((copiedBytes * 1.0 / Wresponse.ContentLength) * 100.0);
                        }
                    }
                }
            }
        }

        public static void UpdateProgress(Double k)
        {

        }
    }
}
