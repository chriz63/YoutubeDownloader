using AngleSharp.Text;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YoutubeDownloader.Helper;
using YoutubeDownloader.Models;
using YoutubeExplode;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos.Streams;

namespace YoutubeDownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IConfiguration Configuration;

        private readonly ObservableCollection<VideoModel> videoList = new ObservableCollection<VideoModel>();

        private readonly DataRetriever retriever = new DataRetriever();

        public Progress<Double> progress;

        public MainWindow(IConfiguration configuration)
        {
            InitializeComponent();
            Configuration = configuration;
            ListVideos.ItemsSource = videoList;
        }

        /// <summary>
        /// void <c>ButtonAdd_Click</c> adds a VideoModel to a <c>ObservableCollection</c> 
        /// and clears the <c>TextBoxUrl</c> after that.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var url = TextBoxUrl.Text;
                VideoModel video = await retriever.ToVideoModel(url);

                videoList.Add(video);
                TextBoxUrl.Clear();
            }
            catch(Exception)
            {
                System.Windows.MessageBox.Show("Invalid YouTube URL", "Error");
            }
            
        }

        /// <summary>
        /// void <c>ButtonRemoveSelected_Click</c> removes selected items from <c>ObservableCollection</c>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonRemoveSelected_Click(object sender, RoutedEventArgs e)
        {
            List<VideoModel> selectedItems = new List<VideoModel>();
            foreach (VideoModel video in ListVideos.SelectedItems) 
            {
                selectedItems.Add(video);
            }
            foreach (VideoModel video in selectedItems) 
            {
                videoList.Remove(video);
            }
        }

        /// <summary>
        /// void <c>ButtonRemoveSelected_Click</c> removes all items from <c>ObservableCollection</c>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonRemoveAll_Click(object sender, RoutedEventArgs e)
        {
            videoList.Clear();
        }

        /// <summary>
        /// void <c>CheckBoxAudio_Checked</c> chceks if <c>CheckBoxAudio</c> is checked 
        /// and if it is then unchecks <c>CheckBoxVideo</c>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxAudio_Checked(object sender, RoutedEventArgs e)
        {
            if (CheckBoxAudio.IsChecked == true) 
            {
                CheckBoxVideo.IsChecked = false;
            }
        }

        /// <summary>
        /// void <c>CheckBoxVideo_Checked</c> chceks if <c>CheckBoxVideo</c> is checked 
        /// and if it is then unchecks <c>CheckBoxAudio</c>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxVideo_Checked(object sender, RoutedEventArgs e)
        {
            if (CheckBoxVideo.IsChecked == true) 
            {
                CheckBoxAudio.IsChecked = false;
            }
        }

        /// <summary>
        /// void <c>ButtonStart_Click</c> starts the downloads
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            YoutubeClient yt = new YoutubeClient();

            if (CheckBoxAudio.IsChecked == false && CheckBoxVideo.IsChecked == false)
            {
                System.Windows.MessageBox.Show("Please select a file format!", "Error");
            }
            else if (TextBoxLocation.Text.Equals(""))
            {
                System.Windows.MessageBox.Show("Please select a file location in Settings!", "Error");
            }
            else if (videoList.Count == 0) 
            {
                System.Windows.MessageBox.Show("Please add videos to the list!", "Error");
            }

            if (CheckBoxAudio.IsChecked == true)
            {
                var counter = 0;
                foreach (var item in videoList)
                {
                    counter++;
                    var streamManifest = await yt.Videos.Streams.GetManifestAsync(item.Url);
                    var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
                    var progress = new Progress<double>(p => ProgressBar.Value = $"{p}".ToDouble());
                    TextProgressBar.Text = $"Downloadig {counter} of {videoList.Count}";
                    await yt.Videos.Streams.DownloadAsync(streamInfo, $"{TextBoxLocation.Text}\\{item.Title}.mp3", progress);
                    ProgressBar.Value = 0;
                }
            }
            else if (CheckBoxVideo.IsChecked == true)
            {
                var counter = 0;
                foreach (var item in videoList)
                {
                    counter++;
                    var fileName = $"{TextBoxLocation.Text}\\{item.Title}.mp4";
                    var progress = new Progress<double>(p => ProgressBar.Value = $"{p}".ToDouble());
                    TextProgressBar.Text = $"Downloadig {counter} of {videoList.Count}";
                    await yt.Videos.DownloadAsync(item.Url, $"{TextBoxLocation.Text}\\{item.Title}.mp4", o => o.SetPreset(ConversionPreset.UltraFast), progress);
                    ProgressBar.Value = 0;
                }
            }
        }

        /// <summary>
        /// void <c>ButtonLocation_Click</c> opens a <c>FolderBrowserDialog</c> and sets the selected folder
        /// to the Value of <c>TextBoxLocation</c>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonLocation_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            var result = dialog.ShowDialog();
            if ( result == System.Windows.Forms.DialogResult.OK ) 
            {
                TextBoxLocation.Text = dialog.SelectedPath;
            }
        }

        /// <summary>
        /// void <c>Hyperlink_RequestNavigate</c> opens a webbrowser with the url from hyperlink
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}
