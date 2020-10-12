using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using YoutubeDownloader.Classes.Decrypt;

namespace YoutubeDownloader.Classes
{
    static class Decrypter
    {
        static Dictionary<string, string> sol = new Dictionary<string, string>();

        static List<string> type = new List<string>();
        static List<int> tValue = new List<int>();

        public static void Decrypt(JObject signatureCipher, VideoInfo SVD)
        {
            Debug.SaveFile(signatureCipher.ToString(), "signatureCipher");

            getFuns(SVD);

            Debug.SaveFile(JObject.Parse(JsonConvert.SerializeObject(sol)).ToString(), "solFile");

            var ss = signatureCipher["s"].ToString();

            Debug.SaveFile(ss.ToString(), "beforeSignatureCipher");

            for (var i = 0; i < type.Count; i++)
            {
                if (type[i] == "Splice")
                {
                    ss = splice(ss, tValue[i]);
                }

                if (type[i] == "Swap")
                {
                    ss = swap(ss, tValue[i]);
                }

                if (type[i] == "Reverse")
                {
                    ss = rev(ss);
                }
            }

            Debug.SaveFile(ss, "afterSignatureCipher");

            var url = signatureCipher["url"].ToString();
            var key = signatureCipher["sp"].ToString();
            // Find existing parameter
            var existingMatch = Regex.Match(url, $"/({Regex.Escape(key)}/?.*?)(?:/|$)");

            // Parameter already set to something
            if (existingMatch.Success)
            {
                var group = existingMatch.Groups[1];

                // Remove existing
                url = url.Remove(group.Index, group.Length);

                // Insert new one
                url = url.Insert(group.Index, $"{key}/{ss}");

            }
            // Parameter hasn't been set yet
            else
            {
                // Assemble new query string
                url = url + '/' + key + '/' + ss;
            }

            Debug.SaveFile(url, "urlsic");

            

        }

        private static string splice(string sc, int pos)
        {
            return sc.Remove(0, pos);
        }

        private static string swap(string sc, int pos)
        {
            var a = sc.ToCharArray();

            var b = a[0];

            a[0] = a[pos % a.Length];

            a[pos % a.Length] = b;

            return new string(a);
        }

        private static string rev(string sc)
        {
            var a = sc.ToCharArray();

            Array.Reverse(a);

            return new string(a);
        }

        private static void getFuns(VideoInfo SVD)
        {

            var link = string.Format("https://www.youtube.com{0}", SVD.jsFile);

            var _root = WebHelper.GetPageSouce(link);

            Debug.SaveFile(_root, "jsfile");

            string TryGetDeciphererFuncBody()
            {
                var funcName = Regex.Match(_root, @"(\w+)=function\(\w+\){(\w+)=\2\.split\(\x22{2}\);.*?return\s+\2\.join\(\x22{2}\)}")
                    .Groups[0]
                    .Value;

                return funcName;
            }

            Debug.SaveFile(TryGetDeciphererFuncBody(), "DeciphererFuncBody");

            string TryGetDeciphererDefinitionBody(string body)
            {
                var objName = Regex.Match(body, "([\\$_\\w]+).\\w+\\(\\w+,\\d+\\);")
                    .Groups[1]
                    .Value;

                var escapedObjName = Regex.Escape(objName);

                return Regex.Match(_root, $@"var\s+{escapedObjName}=\{{(\w+:function\(\w+(,\w+)?\)\{{(.*?)\}}),?\}};", RegexOptions.Singleline)
                    .Groups[0]
                    .Value;
            }

            Debug.SaveFile(TryGetDeciphererDefinitionBody(TryGetDeciphererFuncBody()), "DeciphererDefinitionBody");

            var deciphererFuncBody =
                TryGetDeciphererFuncBody();

            var deciphererDefinitionBody =
                TryGetDeciphererDefinitionBody(deciphererFuncBody);

            // Analyze statements to determine cipher function names
            foreach (var statement in deciphererFuncBody.Split(";"))
            {
                // Get the name of the function called in this statement
                var calledFuncName = Regex.Match(statement, @"\w+(?:.|\[)(\""?\w+(?:\"")?)\]?\(").Groups[1].Value;
                if (string.IsNullOrWhiteSpace(calledFuncName))
                    continue;

                // Slice
                if (Regex.IsMatch(deciphererDefinitionBody, $@"{Regex.Escape(calledFuncName)}:\bfunction\b\([a],b\).(\breturn\b)?.?\w+\."))
                {
                    var index = Regex.Match(statement, @"\(\w+,(\d+)\)").Groups[1].Value;

                    sol.Add("Splice |" + sol.Count, index);

                    type.Add("Splice");
                    tValue.Add(int.Parse(index));
                }

                // Swap
                else if (Regex.IsMatch(deciphererDefinitionBody, $@"{Regex.Escape(calledFuncName)}:\bfunction\b\(\w+\,\w\).\bvar\b.\bc=a\b"))
                {
                    var index = Regex.Match(statement, @"\(\w+,(\d+)\)").Groups[1].Value;

                    sol.Add("Swap |" + sol.Count, index);

                    type.Add("Swap");
                    tValue.Add(int.Parse(index));
                }

                // Reverse
                else if (Regex.IsMatch(deciphererDefinitionBody, $@"{Regex.Escape(calledFuncName)}:\bfunction\b\(\w+\)"))
                {
                    sol.Add("Reverse |" + sol.Count, "null");

                    type.Add("Reverse");
                    tValue.Add(-1);
                }
            }


        }
    }
}
