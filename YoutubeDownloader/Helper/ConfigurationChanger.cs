using System;
using System.Configuration;

namespace YoutubeDownloader.Helper
{
    /// <summary>
    /// class <c>ConfigurationChanger</c> includes a method to change keys with their given values in the App.config file
    /// </summary>
    public class ConfigurationChanger : IConfigurationChanger
    {
        /// <summary>
        /// bool <c>Change</c> changes a value with their given key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Change(string key, string value)
        {
            try 
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings[key].Value = value;
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
                return true;
            }
            catch (Exception ex) 
            {
                System.Windows.MessageBox.Show(ex.Message);
                return false;
            }
            
        }
    }
}
