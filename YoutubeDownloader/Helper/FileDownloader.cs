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

        /// <summary>
        /// Task <c>Start</c> starts the download of ffmpeg
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// void <c>DownloadProgressChanged</c> sets progress and text to progressbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            var megabytesReceived = e.BytesReceived / 1048;
            var totalMegabytesToReceive = e.TotalBytesToReceive / 1048;
            ProgressBar.Value = e.ProgressPercentage;
            TextBlock.Text = $"Downloaded {megabytesReceived} from {totalMegabytesToReceive} Megabytes";
        }
    }
}
