using System;
using System.ComponentModel;
using System.Threading.Tasks;
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

    }

}

