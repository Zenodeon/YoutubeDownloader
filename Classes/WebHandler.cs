using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Rectangle = System.Windows.Shapes.Rectangle;

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
    }
}
