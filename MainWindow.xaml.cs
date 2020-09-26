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
using YoutubeDownloader.Classes;



namespace YoutubeDownloader
{
    public partial class MainWindow : Window
    {
        Progress<IProgressData> progress = new Progress<IProgressData>();
        IProgressData progressData = new IProgressData();

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
                await LinkHandler.DownloadVideo(videoURL.Text, progress);
            }
        }

        private async void videoURL_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (LinkHandler.IsValidLink(videoURL.Text))
            {
                debug.Content = "ValidLink";

                vaildLink = true;
               
                BitmapImage image = await LinkHandler.GetThumbnailAsync(videoURL.Text, progress);

                VideoThumbnail_Image.Source = image;

            }
            else
            {
                debug.Content = "NotValidLink";
                vaildLink = false;
            }
        }

        private void ProgressUpdater(object sender, IProgressData e)
        {
            Bar2.Value = e.Percent;
        }

        private void debug_Button_Click(object sender, RoutedEventArgs e)
        {
            var videoID = LinkHandler.GetVideoID(videoURL.Text);
            string videoInfoLink = string.Format("https://www.youtube.com/get_video_info?video_id={0}&el=detailpage&hl=en", videoID);

            var rawVideoInfo = WebHandler.GetPageSouce(videoInfoLink);

            Debug.SaveFile(rawVideoInfo, "RawVideoInfo");

            var videoInfo = new Dictionary<string, string>();

            foreach (string vp in Regex.Split(rawVideoInfo, "&"))
            {
                string[] strings = Regex.Split(vp, "=");

                videoInfo.Add(strings[0], strings.Length == 2 ? HttpUtility.UrlDecode(strings[1]) : string.Empty);
            }

            JObject videoInfoJson = JObject.Parse(JsonConvert.SerializeObject(videoInfo).ToString());

            Debug.SaveFile(videoInfoJson.ToString(), "VideoInfoJson");

            JObject videoInfoPlayerJson = JObject.Parse(videoInfoJson["player_response"].ToString());

            Debug.SaveFile(videoInfoPlayerJson.ToString(), "VideoInfoPlayerJson");
        }
    }

}

