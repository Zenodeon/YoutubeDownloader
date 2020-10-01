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
            VideoInfo SVD = YoutubeHelper.GetVideoData((string)e.Argument);

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
            videoQuailty_List.Items.Add(test++);

        }
    }
    
}

