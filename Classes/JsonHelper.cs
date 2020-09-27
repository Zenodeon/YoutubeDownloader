using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace YoutubeDownloader.Classes
{
    static class JsonHelper
    {
        public static JObject ConvertToJson(string rawData)
        {
            var dataDictionary = new Dictionary<string, string>();

            foreach (string data in Regex.Split(rawData, "&"))
            {
                string[] strings = Regex.Split(data, "=");

                dataDictionary.Add(strings[0], strings.Length == 2 ? HttpUtility.UrlDecode(strings[1]) : string.Empty);
            }

            return JObject.Parse(JsonConvert.SerializeObject(dataDictionary).ToString());
        }
    }
}
