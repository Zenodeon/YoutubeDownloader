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

namespace YoutubeDownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MediaPlayer player = new MediaPlayer();

        string testVideoPath = "C:/Users/Admin/Desktop/magnets.mp4";

        string videoURL = "https://www.youtube.com/watch?v=vqiWcgaLNYY";

        string savePath = "C:/Users/Admin/Downloads/YoutubeDownloader/";

        public MainWindow()
        {
            InitializeComponent();

            //TextBox2.Text = System.Web.HttpUtility.UrlDecode(videoURL); 
            // player.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {            
            JObject VJson = GetJsonFromUrl(videoURL);
            //TextBox2.Text = DecodeUrl(videoURL);

            SaveFile(VJson.ToString(), "YoutubeResponse");
        }

        public JObject GetJsonFromUrl(string url)
        {
            string htmlfile;
            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                htmlfile = client.DownloadString(url);
            }

            Regex dataRegex = new Regex(@"ytplayer\.config\s*=\s*(\{.+?\});", RegexOptions.Multiline);

            string extractedJson = dataRegex.Match(htmlfile).Result("$1");
            return JObject.Parse(extractedJson);
        }
      
        public void SaveFile(string data, string filename)
        {
            File.WriteAllText(savePath + filename, data);
        }
    }

}


