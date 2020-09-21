using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace YoutubeDownloader.Classes
{
    public static class Debug
    {
        private static string savePath = "C:/Users/Admin/Downloads/YoutubeDownloader/";
        public static void SaveFile(string data, string filename, string extension = null)
        {
            File.WriteAllText(savePath + filename + extension, data);
        }
        public static void SaveFile(string[] data, string filename, string extension = null)
        {
            File.WriteAllLines(savePath + filename + extension, data);
        }
    }
}
