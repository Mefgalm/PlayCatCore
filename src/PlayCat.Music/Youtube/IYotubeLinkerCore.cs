using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlayCat.Music.Youtube
{
    public interface IYotubeLinkerCore
    {
        Task<IEnumerable<VideoInfo>> GetDownloadUrlsAsync(string videoUrl);
    }
}