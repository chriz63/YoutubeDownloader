using System.Threading.Tasks;

namespace YoutubeDownloader.Helper
{
    public interface IFileDownloader
    {
        public Task Start();
    }
}
