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
using System.Web;
using YoutubeDownloader.Classes;


namespace YoutubeDownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MediaPlayer player = new MediaPlayer();

        //string testVideoPath = "C:/Users/Admin/Desktop/magnets.mp4";
        //string videoURL = "https://www.youtube.com/watch?v=xzIADWo9-bc&hl=en";
        //string videoURL = "https://www.youtube.com/watch?v=xOWH46e-p8M&hl=en";
        string videoURL = "https://www.youtube.com/watch?v=wWQPnhG0xHU";
        //string videoURL = "https://www.youtube.com/embed/xOWH46e-p8M?hl=en";
        string videoID = "xOWH46e-p8M";

        public MainWindow()
        {
            InitializeComponent();

            // player.Show();

            TextBox2.Text = LinkManager.GetVideoID(videoURL); ;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            Download.Video(videoURL);

                   
        }

    }

}


