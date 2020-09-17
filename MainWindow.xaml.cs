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
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using YoutubeDownloader.Class;

namespace YoutubeDownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MediaPlayer player = new MediaPlayer();

        //string testVideoPath = "C:/Users/Admin/Desktop/magnets.mp4";

        string videoURL = "https://www.youtube.com/watch?v=xOWH46e-p8M";

        

        public MainWindow()
        {
            InitializeComponent();

            string videoId = "xOWH46e-p8M";
            string sts = null;
            var eurl = WebUtility.HtmlEncode($"https://youtube.googleapis.com/v/{videoId}");

            var url = $"https://youtube.com/get_video_info?video_id={videoId}&el=embedded&eurl={eurl}&hl=en&sts={sts}";

            TextBox2.Text = url; 
            // player.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            JsonResponse.GetJsonResponse(videoURL);

            JObject VJson = JsonResponse.GetResponse();

        }


    }

}


