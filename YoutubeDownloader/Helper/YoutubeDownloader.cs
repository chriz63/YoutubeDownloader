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
