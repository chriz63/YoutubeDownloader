﻿using System;
using System.IO;
using System.Windows;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Configuration;

using YoutubeExplode;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos.Streams;

using YoutubeDownloader.Helper;
using YoutubeDownloader.Models;


/*
 * TODO
 * 
 * + remove appsettings.json switch to app.config to store store changes 
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

        private readonly DataRetriever retriever = new DataRetriever();

        public Progress<Double> progress;

        public MainWindow()
        {
            InitializeComponent();
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
            try 
            {
                YoutubeClient yt = new YoutubeClient();

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
                        var counter = 0;
                        foreach (var item in videoList)
                        {
                            string videoTitle = item.Title.Replace("/", "");
                            counter++;
                            var streamManifest = await yt.Videos.Streams.GetManifestAsync(item.Url);
                            var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
                            var progress = new Progress<double>(p => ProgressBar.Value = p);
                            TextProgressBar.Text = $"Downloadig {counter} of {videoList.Count}";
                            await yt.Videos.Streams.DownloadAsync(streamInfo, $"{TextBoxLocation.Text}\\{videoTitle}.mp3", progress);
                        }
                        TextProgressBar.Text = $"{counter} of {videoList.Count} successfully downloaded";
                    }
                    else if (CheckBoxVideo.IsChecked == true)
                    {
                        var counter = 0;
                        foreach (var item in videoList)
                        {
                            counter++;
                            var fileName = $"{TextBoxLocation.Text}\\{item.Title}.mp4";
                            var progress = new Progress<double>(p => ProgressBar.Value = p);
                            TextProgressBar.Text = $"Downloadig {counter} of {videoList.Count}";
                            await yt.Videos.DownloadAsync(item.Url, $"{TextBoxLocation.Text}\\{item.Title}.mp4", o => o.SetPreset(ConversionPreset.UltraFast), progress);
                        }
                        TextProgressBar.Text = $"{counter} of {videoList.Count} successfully downloaded";
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("Directory not exists, please check file location", "Error");
                }
            }
            catch (Exception ex)
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
