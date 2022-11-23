using System;
using Ionic.Zip;
using System.Configuration;
using System.Windows.Controls;
using System.IO;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Windows.Media;

namespace YoutubeDownloader.Helper
{
    public class FileExtractor : IFileExtractor
    {
        ProgressBar ProgressBar;
        TextBlock TextBlock;

        NameValueCollection config;
        string ffmpegDestination;
        string extractionDestination;

        EnvironmentVariableChanger envChanger = new EnvironmentVariableChanger();

        public FileExtractor(ProgressBar progressBar, TextBlock textBlock) 
        {
            ProgressBar = progressBar;
            TextBlock = textBlock;

            config = ConfigurationManager.AppSettings;
            ffmpegDestination = config["FFmpegDestination"] + "\\ffmpeg.zip";
            extractionDestination = config["FFmpegDestination"] + "\\";
        }

        public async Task Start()
        {
            try 
            {
                ZipFile zip = new ZipFile(ffmpegDestination);
                zip.ExtractProgress += new EventHandler<ExtractProgressEventArgs>(ZipExtractProgressChanged);
                TextBlock.Text = "Extracting zip archive";
                zip.ExtractAll(extractionDestination, ExtractExistingFileAction.OverwriteSilently);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error");
            }
        }

        public async Task RenameDirectory()
        {
            try
            {   
                if (Directory.Exists($"{extractionDestination}ffmpeg-5.1.2-essentials_build"))
                {
                    Directory.Move($"{extractionDestination}ffmpeg-5.1.2-essentials_build", $"{extractionDestination}ffmpeg");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error!");
            }
        }

        private async void ZipExtractProgressChanged(object sender, ExtractProgressEventArgs e)
        {
            long totalBytesToTransfer = 0;
            long bytesTransferred = 0;

            if (e.TotalBytesToTransfer > 0)
            {
                bytesTransferred += e.BytesTransferred;
                totalBytesToTransfer += e.TotalBytesToTransfer;
                ProgressBar.Value = Convert.ToInt32(100 * e.BytesTransferred / e.TotalBytesToTransfer);
            }
            
            if (e.EntriesExtracted > 0)
            {
                Console.WriteLine($"Ectracted: {e.EntriesExtracted}, Total: {e.EntriesTotal}");
                if (e.EntriesExtracted == e.EntriesTotal)
                {
                    await Task.Delay(1000);
                    await RenameDirectory();
                    await Task.Delay(1000);
                    //envChanger.Add();
                }
            }
        }
    }
}
