using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using Microsoft.Windows.Themes;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Media;
using System.IO.Packaging;

using YoutubeExtractor;

namespace YoutubeDownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MediaPlayer player = new MediaPlayer();

        String testVideoPath = "C:/Users/Admin/Desktop/magnets.mp4";



        public HttpContent respt;

        public MainWindow()
        {
            InitializeComponent();


            // player.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //player.Play(testVideoPath);

            //player.PlayBack.Source = new Uri(testVideoPath);
            string url = "https://www.youtube.com/watch?v=vqiWcgaLNYY";
            Test(url);

        }


        public void Test(string url)
        {
            IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(url);

            VideoInfo video = videoInfos.First(info => info.VideoType == VideoType.Mp4 && info.Resolution == 1080);
            if (video.RequiresDecryption)
            {
                DownloadUrlResolver.DecryptDownloadUrl(video);
            }
            var videoDownloader = new VideoDownloader(video, System.IO.Path.Combine("D:/Downloads", video.Title + video.VideoExtension));
            videoDownloader.DownloadProgressChanged += (sender, args) => Console.WriteLine(args.ProgressPercentage);

            videoDownloader.Execute();
        }

    }

}


