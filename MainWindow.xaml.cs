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

                BitmapImage image = await LinkHandler.GetThumbnailAsync(videoURL.Text, Bar);

                VideoThumbnail_Image.Source = image;

            }
            else
            {
                debug.Content = "NotValidLink";
            }
        }
    }

}

