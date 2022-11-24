using System;

namespace YoutubeDownloader.Helper
{
    /// <summary>
    /// class <c>EnvironmentVariableChanger</c> contains method to add a windows environment variable
    /// </summary>
    public class EnvironmentVariableChanger : IEnvironmentVariableChanger
    {
        /// <summary>
        /// void <c>Add</c> adds the path to ffmpeg to windows environment variables
        /// </summary>
        public void Add()
        {
            try
            {
                var config = System.Configuration.ConfigurationManager.AppSettings;
                var ffmpegDestination = config["FFmpegDestination"].Replace("\\", "");
                var path = System.Environment.GetEnvironmentVariable("Path");
                var newPath = path + @$"{ffmpegDestination}\ffmpeg\bin";

                Environment.SetEnvironmentVariable("Path", newPath, EnvironmentVariableTarget.User);
            }
            catch (Exception ex) 
            {
                System.Windows.MessageBox.Show(ex.Message, "Error!");
            }
        }
    }
}
