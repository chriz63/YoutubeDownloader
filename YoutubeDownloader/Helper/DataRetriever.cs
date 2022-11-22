using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDownloader.Models;

using YoutubeExplode;
using YoutubeExplode.Videos;

namespace YoutubeDownloader.Helper
{
    public class DataRetriever : IDataRetriever
    {
        YoutubeClient yt = new YoutubeClient();

        /// <summary>
        /// Task <c>ToVideoModel</c> retrieves data from a Youtube URL, and returns them in a VideoModel
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<VideoModel> ToVideoModel(string url)
        {
            var video = await yt.Videos.GetAsync(url);
            VideoModel videoData = new VideoModel { Title=video.Title, Duration=video.Duration.ToString(), Url=url };
            return videoData;
        }
    }
}
