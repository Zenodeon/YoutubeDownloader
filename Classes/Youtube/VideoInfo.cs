using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace YoutubeDownloader.Classes
{
    class VideoInfo
    {
        public JObject rawVideoInfo { get; set; }
        public JObject PlayerResponse { get; set; }
        public JArray Formats { get; set; }
        public JArray AdaptiveFormats { get; set; }
        public bool requiresDecrytion { get; set; }
        public string VideoUrl { get; set; }
        public string VideoName { get; set; }
    }
}
