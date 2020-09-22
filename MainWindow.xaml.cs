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
using System.Timers;


namespace YoutubeDownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //string testVideoPath = "C:/Users/Admin/Desktop/magnets.mp4";
        //string videoURL = "https://www.youtube.com/watch?v=xzIADWo9-bc&hl=en";
        //string videoURL = "https://www.youtube.com/watch?v=xOWH46e-p8M&hl=en";
        string testvideoURL = "https://www.youtube.com/watch?v=wWQPnhG0xHU";
        //string videoURL = "https://www.youtube.com/embed/xOWH46e-p8M?hl=en";
        string videoID = "xOWH46e-p8M";

        public MainWindow()
        {
            InitializeComponent();

            //debug.Text = LinkManager.GetVideoID(testvideoURL); ;

            //Bar.Width = 500 / 2;

            Bar.Width = 0;
        }

        private void Download_Button_Click(object sender, RoutedEventArgs e)
        {
            //Download.Video(testvideoURL);

        }

        private void videoURL_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(LinkHandler.IsValidLink(videoURL.Text))
            {
                debug.Content = "ValidLink";

                VideoThumbnail_Image.Source = LinkHandler.GetThumbnail(videoURL.Text);
            }
            else
            {
                debug.Content = "NotValidLink";
            }
        }
    }

}

