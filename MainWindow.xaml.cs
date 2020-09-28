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
        Progress<DownloadProgressData> progress = new Progress<DownloadProgressData>();
        DownloadProgressData progressData = new DownloadProgressData();

        private bool vaildLink = false;

        public MainWindow()
        {
            InitializeComponent();
            
            progress.ProgressChanged += ProgressUpdater;
        }

        private async void Download_Button_Click(object sender, RoutedEventArgs e)
        {
            if (vaildLink)
            {
                await WebHelper.DownloadVideoAsync(videoURL.Text, progress);
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


                UpdateList(videoURL.Text);
            }
            else
            {
                debug.Content = "NotValidLink";
                vaildLink = false;
            }
        }

        private void UpdateList(string url)
        {
            VideoInfo vi = new VideoInfo();
            vi = YoutubeHelper.GetVideoData(url);

            //JArray formats = vi.Formats;
            JArray formats = vi.AdaptiveFormats;


            foreach(var info in formats)
            {
                if (info["mimeType"].ToString() != "video/webm; codecs=\"vp9\"")
                {
                    videoQuailty_List.Items.Add(info["qualityLabel"]);
                }
            }
        }

        private void ProgressUpdater(object sender, DownloadProgressData e)
        {
            Bar2.Value = e.Percent;
        }

        private void debug_Button_Click(object sender, RoutedEventArgs e)
        {
            videoQuailty_List.Items.Add("bruh");
            
        }

        private void videoQuailty_List_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBox lb = sender as ListBox;
            if (lb.SelectedIndex != -1)
            {
                MessageBox.Show(lb.SelectedIndex + "");
            }
        }
    }
    
}

