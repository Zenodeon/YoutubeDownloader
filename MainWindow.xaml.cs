using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Input;
using YoutubeDownloader.Classes;
using YoutubeDownloader.Classes.Youtube;

namespace YoutubeDownloader
{
    public partial class MainWindow : Window
    {
        BackgroundWorker lister = new BackgroundWorker();

        Progress<DownloadProgressData> progress = new Progress<DownloadProgressData>();
        //DownloadProgressData progressData = new DownloadProgressData();

        private bool vaildLink = false;

        JArray AvailableFormats = new JArray();
        JToken SelectedFormat;

        VideoInfo SVD;

        public MainWindow()
        {
            InitializeComponent();

            lister.DoWork += GetAvailableFormats;
            lister.WorkerReportsProgress = true;
            lister.ProgressChanged += UpdateFormatList;

            progress.ProgressChanged += ProgressUpdater;

        }


        private async void Download_Button_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedFormat != null)
            {
                await WebHelper.DownloadVideoAsync(JObject.Parse(SelectedFormat.ToString()), SVD, progress);
            }
        }

        private async void videoURL_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (VideoLink.IsValidLink(videoURL.Text))
            {
                debug.Content = "ValidLink";

                vaildLink = true;

                BitmapImage image = await WebHelper.GetThumbnailAsync(videoURL.Text, progress);

                VideoThumbnail_Image.Source = image;

                AvailableFormats.Clear();
                videoQuailty_List.Items.Clear();
                lister.RunWorkerAsync(argument: videoURL.Text);
            }
            else
            {
                debug.Content = "NotValidLink";
                vaildLink = false;
            }
        }

        private void GetAvailableFormats(object sender, DoWorkEventArgs e)
        {
            SVD = YoutubeHelper.GetVideoData((string)e.Argument);

            //JArray formats = vi.Formats;
            JArray formats = SVD.AdaptiveFormats;
            JArray listedFormats = new JArray();
            int pg = 0;
            foreach (var info in formats)
            {
                if (info["mimeType"].ToString() != "video/webm; codecs=\"vp9\"")
                {

                    listedFormats.Add(info);

                    lister.ReportProgress(pg, info);
                    pg++;
                }
            }

        }

        private void UpdateFormatList(object sender, ProgressChangedEventArgs e)
        {
            JToken format = (JToken)e.UserState;
            if (format["qualityLabel"] != null)
            {
                videoQuailty_List.Items.Add(format["qualityLabel"]);
                AvailableFormats.Add(format);
            }
        }

        private void ProgressUpdater(object sender, DownloadProgressData e)
        {
            Bar2.Value = e.Percent;
        }


        private void videoQuailty_List_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBox lb = sender as ListBox;
            if (lb.SelectedIndex != -1)
            {
                debug.Content = AvailableFormats[lb.SelectedIndex]["qualityLabel"] + "";
                SelectedFormat = AvailableFormats[lb.SelectedIndex];
            }
        }

        //debug
        float test = 0;
        private void debug_Button_Click(object sender, RoutedEventArgs e)
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
                    Debug.SaveFile(Regex.Match(statement, @"\(\w+,(\d+)\)").ToString(), "SliceFull");
                    Debug.SaveFile(index, "Slice");
                }

                // Swap
                else if (Regex.IsMatch(deciphererDefinitionBody, $@"{Regex.Escape(calledFuncName)}:\bfunction\b\(\w+\,\w\).\bvar\b.\bc=a\b"))
                {
                    var index = Regex.Match(statement, @"\(\w+,(\d+)\)").Groups[1].Value;
                    Debug.SaveFile(Regex.Match(statement, @"\(\w+,(\d+)\)").ToString(), "SwapFull");
                    Debug.SaveFile(index, "Swap");
                }

                // Reverse
                else if (Regex.IsMatch(deciphererDefinitionBody, $@"{Regex.Escape(calledFuncName)}:\bfunction\b\(\w+\)"))
                {
                    //Debug.SaveFile(index, "Slice");
                }
            }


        }
    }

}

