using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace YoutubeDownloader.Helper
{
    public class FileDownloader : IFileDownloader
    {
        ProgressBar ProgressBar;
        TextBlock TextBlock;

        public FileDownloader(ProgressBar progressBar, TextBlock textBlock) 
        {
            ProgressBar = progressBar;
            TextBlock = textBlock;
        }

        public async Task Start()
        {
            var config = ConfigurationManager.AppSettings;
            var ffmpegUrl = config["FFmpegURL"];
            var ffmpegDestination = config["FFmpegDestination"] + "\\ffmpeg.zip";

            WebClient wc = new WebClient();
            wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressChanged);
            TextBlock.Text = "Downloading FFmpeg";
            await wc.DownloadFileTaskAsync(new Uri(ffmpegUrl), ffmpegDestination);
        }

        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            ProgressBar.Value = e.ProgressPercentage;
        }
    }
}
