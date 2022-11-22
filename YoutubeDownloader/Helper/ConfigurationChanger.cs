using System;
using System.Configuration;

namespace YoutubeDownloader.Helper
{
    public class ConfigurationChanger : IConfigurationChanger
    {
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
