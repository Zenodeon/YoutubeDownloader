using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace YoutubeDownloader.Classes
{
    static class HttpHandler
    {
        public static string GetPageSouce(string url)
        {
            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;

                return client.DownloadString(url);
            }
        }
    }
}
