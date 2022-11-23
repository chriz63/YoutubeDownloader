using System;
using System.IO;
using System.Windows;
using System.Diagnostics;
using System.Configuration;
using System.Windows.Forms;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using YoutubeExplode;

using YoutubeDownloader.Helper;
using YoutubeDownloader.Models;



/*
 * TODO
 * 
 * + implement ffmpeg installer that installs ffmpeg to given directory and installs in windows environment vars
 * 
 */

namespace YoutubeDownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<VideoModel> videoList = new ObservableCollection<VideoModel>();

        private readonly IDataRetriever _dataRetriever;
        private readonly IConfigurationChanger _configurationChanger;

        public Progress<Double> progress;

        public MainWindow(IConfigurationChanger configurationChanger, IDataRetriever dataRetriever)
        {
            InitializeComponent();
            ListVideos.ItemsSource = videoList;
            TextBoxLocation.Text = ConfigurationManager.AppSettings["DownloadDestination"];
            TextBoxFfmpegLocation.Text = ConfigurationManager.AppSettings["FFmpegDestination"];

            _dataRetriever = dataRetriever;
            _configurationChanger= configurationChanger;
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
                VideoModel video = await _dataRetriever.ToVideoModel(url);

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
        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                YoutubeClient yt = new YoutubeClient();

                Helper.YoutubeDownloader youtubeDownloader = new Helper.YoutubeDownloader
                    (videoList, ProgressBar, TextProgressBar, TextBoxLocation, CheckBoxAudio, CheckBoxVideo);

                if (CheckBoxAudio.IsChecked == false && CheckBoxVideo.IsChecked == false)
                {
                    System.Windows.MessageBox.Show("Please select a file format!", "Error");
                }
                else if (videoList.Count == 0)
                {
                    System.Windows.MessageBox.Show("Please add videos to the list!", "Error");
                }


                if (Directory.Exists(TextBoxLocation.Text))
                {
                    if (CheckBoxAudio.IsChecked == true)
                    {
                        youtubeDownloader.DownloadAudio();
                    }
                    else if (CheckBoxVideo.IsChecked == true)
                    {
                        youtubeDownloader.DownloadVideo();
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("Directory not exists, please check file location", "Error");
                }
            }
            catch(Exception ex) 
            {
                System.Windows.MessageBox.Show(ex.Message, "Error");
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
                _configurationChanger.Change("DownloadDestination", dialog.SelectedPath);
            }
            Console.WriteLine(ConfigurationManager.AppSettings["DownloadDestination"]);
        }

        private void ButtonFfmpegLocation_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            var result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                TextBoxFfmpegLocation.Text = dialog.SelectedPath;
                _configurationChanger.Change("FFmpegDestination", dialog.SelectedPath);
            }
            Console.WriteLine(ConfigurationManager.AppSettings["FFmpegDestination"]);
        }

        private async void ButtonFfmpegInstall_Click(object sender, RoutedEventArgs e)
        {
            var config = ConfigurationManager.AppSettings;
            var ffmpegUrl = config["FFmpegURL"];
            var ffmpegDestination = config["FFmpegDestination"] + "\\ffmpeg.zip";
            try 
            {
                if ( ffmpegUrl != null && ffmpegDestination != null && File.Exists(ffmpegDestination) == false) 
                {
                    FileDownloader fileDownloader = new FileDownloader(ProgressBarFfmpeg, TextProgressBarFfmpeg);
                    await fileDownloader.Start();
                }
                if (File.Exists(ffmpegDestination)) 
                {
                    FileExtractor fileExtractor = new FileExtractor(ProgressBarFfmpeg, TextProgressBarFfmpeg);
                    await fileExtractor.Start();
                }
            }
            catch (Exception ex) 
            {
                System.Windows.MessageBox.Show(ex.ToString(), "Error");
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
