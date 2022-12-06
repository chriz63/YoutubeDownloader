namespace YoutubeDownloader.Helper
{
    public interface IConfigurationChanger
    {
        public bool Change(string key, string value);
    }
}
