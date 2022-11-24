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
    /// <summary>
    /// class <c>FileExtractor</c> contains methods to extract a zip file
    /// </summary>
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

        /// <summary>
        /// Task <c>Start</c> starts the file extracting and sends the progress to an event handler
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// void <c>RenameDirectory</c> ranames the extracted directory
        /// </summary>
        public void RenameDirectory()
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

        /// <summary>
        /// void <c>ZipExtractProgressChanged</c> sets the value of the progressbar,
        /// when extracting is finished, mehtod renames the directory of the extracted zip file
        /// and adds the full path of directory to windows environment variable user path
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ZipExtractProgressChanged(object sender, ExtractProgressEventArgs e)
        {
            if (e.TotalBytesToTransfer > 0)
            {
                ProgressBar.Value = Convert.ToInt32(100 * e.BytesTransferred / e.TotalBytesToTransfer);
            }
            
            if (e.EntriesExtracted > 0)
            {
                TextBlock.Text = ($"Extracted {e.EntriesExtracted} from {e.EntriesTotal} files");
                if (e.EntriesExtracted == e.EntriesTotal)
                {
                    await Task.Delay(1000);
                    TextBlock.Text = "Renaming ffmpeg directory";
                    RenameDirectory();
                    await Task.Delay(1000);
                    TextBlock.Text = "Adding ffmpeg to path";
                    envChanger.Add();
                    await Task.Delay(1000);
                    TextBlock.Text = "FFmpeg was successfully installed";
                }
            }
        }
    }
}
