using System.Threading.Tasks;

using YoutubeDownloader.Models;

namespace YoutubeDownloader.Helper
{
    public interface IDataRetriever
    {
        public Task<VideoModel> ToVideoModel(string url);
    }
}
