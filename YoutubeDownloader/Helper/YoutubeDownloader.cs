using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using YoutubeDownloader.Models;
using YoutubeExplode;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos.Streams;

namespace YoutubeDownloader.Helper
{
    /// <summary>
    /// class <cYoutubeDownloader</c> contains methods to download videos as audio or video from youtube
    /// </summary>
    public class YoutubeDownloader : IYoutubeDownloader
    {

        ProgressBar ProgressBar;
        TextBlock TextProgressBar;
        TextBox TextBoxLocation;
        CheckBox CheckBoxAudio;
        CheckBox CheckBoxVideo;

        ObservableCollection<VideoModel> VideoList;

        YoutubeClient yt = new YoutubeClient();

        public YoutubeDownloader(ObservableCollection<VideoModel> videoList, ProgressBar progressBar, TextBlock textProgressBar, TextBox textBoxLocation, CheckBox checkBoxAudio, CheckBox checkBoxVideo) 
        {
            ProgressBar = progressBar;
            TextProgressBar = textProgressBar;
            TextBoxLocation = textBoxLocation;
            CheckBoxAudio = checkBoxAudio;
            CheckBoxVideo = checkBoxVideo;
            VideoList = videoList;
        }

        /// <summary>
        /// void <c>DownloadAudio</c> downloads a video from youtube and saves it as mp3 file
        /// </summary>
        public async void DownloadAudio()
        {
            try
            {
                var counter = 0;
                foreach (var item in VideoList)
                {
                    string videoTitle = item.Title.Replace("/", "");
                    counter++;
                    var streamManifest = await yt.Videos.Streams.GetManifestAsync(item.Url);
                    var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
                    var progress = new Progress<double>(p => ProgressBar.Value = p);
                    TextProgressBar.Text = $"Downloadig {counter} of {VideoList.Count}";
                    await yt.Videos.Streams.DownloadAsync(streamInfo, $"{TextBoxLocation.Text}\\{videoTitle}.mp3", progress);
                }
                TextProgressBar.Text = $"{counter} of {VideoList.Count} successfully downloaded";  
            }
            catch (Exception ex)
            {

                System.Windows.MessageBox.Show(ex.Message, "Error");
            }
        }

        /// <summary>
        /// void <c>DownloadVideo</c> downloads a video from youtube and saves it as mp4 file
        /// </summary>
        public async void DownloadVideo()
        {
            try
            {
                var counter = 0;
                foreach (var item in VideoList)
                {
                    counter++;
                    var fileName = $"{TextBoxLocation.Text}\\{item.Title}.mp4";
                    var progress = new Progress<double>(p => ProgressBar.Value = p);
                    TextProgressBar.Text = $"Downloadig {counter} of {VideoList.Count}";
                    await yt.Videos.DownloadAsync(item.Url, $"{TextBoxLocation.Text}\\{item.Title}.mp4", o => o.SetPreset(ConversionPreset.UltraFast), progress);
                }
                TextProgressBar.Text = $"{counter} of {VideoList.Count} successfully downloaded";
                    
            }
            catch (Exception ex)
            {

                System.Windows.MessageBox.Show(ex.Message, "Error");
            }
        }
    }
}
