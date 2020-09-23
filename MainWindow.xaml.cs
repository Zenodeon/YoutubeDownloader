using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using YoutubeDownloader.Classes;



namespace YoutubeDownloader
{
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();

            Bar.Width = 0;
        }

        private void Download_Button_Click(object sender, RoutedEventArgs e)
        {
            //Download.Video(testvideoURL);

        }

        private async void videoURL_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (LinkHandler.IsValidLink(videoURL.Text))
            {
                debug.Content = "ValidLink";

                BitmapImage image = await getThumbnail(videoURL.Text);

                VideoThumbnail_Image.Source = image;

            }
            else
            {
                debug.Content = "NotValidLink";
            }
        }

        private async Task<BitmapImage> getThumbnail(string url)
        {
            BitmapImage thumbnail = null;
            Double progress = 0;
            bool alreadystarted = false;

            while (progress < 100.0)
            {
                if (!alreadystarted)
                {
                    alreadystarted = true;
                    thumbnail = await LinkHandler.GetThumbnailAsync(url);
                }
                progress = WebHandler.Progress;
                debug.Content = progress;
            }
            
            return thumbnail;
        }


    }

}

