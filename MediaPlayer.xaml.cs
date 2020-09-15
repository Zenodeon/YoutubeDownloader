using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace YoutubeDownloader
{
    /// <summary>
    /// Interaction logic for MediaPlayer.xaml
    /// </summary>
    /// 
 
    public partial class MediaPlayer : Window
    {
        public MediaPlayer()
        {
            InitializeComponent();
        }

        public void Play(string videoPath)
        {
            PlayBack.Source = new Uri(videoPath);


        }
    }
}
