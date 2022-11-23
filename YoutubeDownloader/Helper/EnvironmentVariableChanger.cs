using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownloader.Helper
{
    public class EnvironmentVariableChanger : IEnvironmentVariableChanger
    {
        public void Add()
        {
            try
            {
                var config = System.Configuration.ConfigurationManager.AppSettings;
                var ffmpegDestination = config["FFmpegDestination"].Replace("\\", "");
                var scope = System.Environment.GetEnvironmentVariable("Path");
                Console.WriteLine(ffmpegDestination);
                System.Environment.SetEnvironmentVariable("FFmpeg", @$"{ffmpegDestination}\ffmpeg\bin", EnvironmentVariableTarget.Machine);
            }
            catch (Exception ex) 
            {
                System.Windows.MessageBox.Show(ex.Message, "Error!");
            }
        }
    }
}
