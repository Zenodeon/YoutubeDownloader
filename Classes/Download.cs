using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownloader.Classes
{
    static class Download
    {
        public static bool cancelDownloads = false;
        public static void DownloadContent(String url, string fileName, string savePath, IProgress<IProgressData> progress)
        {
            IProgressData Data = new IProgressData();

            var request = (HttpWebRequest)WebRequest.Create(url);

            using (WebResponse Wresponse = request.GetResponse())
            {
                using (Stream source = Wresponse.GetResponseStream())
                {
                    using (FileStream target = File.Open(savePath + fileName, FileMode.Create, FileAccess.Write))
                    {
                        var buffer = new byte[1024];

                        int bytes;
                        int copiedBytes = 0;

                        while (!cancelDownloads && (bytes = source.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            target.Write(buffer, 0, bytes);

                            copiedBytes += bytes;

                            Data.Percent = ((copiedBytes * 1.0 / Wresponse.ContentLength) * 100.0);

                            progress.Report(Data);
                        }
                    }
                }
            }
        }
    }
}
