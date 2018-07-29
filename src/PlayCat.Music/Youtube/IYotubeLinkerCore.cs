using System.Collections.Generic;
using System.Threading.Tasks;

namespace YotubeLinkerCore
{
    public interface IYotubeLinkerCore
    {
        Task<IEnumerable<VideoInfo>> GetDownloadUrlsAsync(string videoUrl);
    }
}