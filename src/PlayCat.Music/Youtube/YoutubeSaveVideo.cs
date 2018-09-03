using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PlayCat.Helpers;

namespace PlayCat.Music.Youtube
{
    public class YoutubeSaveVideo : ISaveVideo
    {
        private const string YoutuberRegexp = @"^(http(s)??\:\/\/)?(www\.)?(youtube\.com\/watch\?v=[\.A-Za-z0-9_\?=&-]+|youtu\.be\/[A-Za-z0-9_-]+)$";        

        private readonly IFileResolver     _fileResolver;
        private readonly IYotubeLinkerCore _yotubeLinkerCore;

        public YoutubeSaveVideo(IFileResolver fileResolver, IYotubeLinkerCore yotubeLinkerCore)
        {
            _fileResolver     = fileResolver;
            _yotubeLinkerCore = yotubeLinkerCore;
        }        

        public async Task<IFile> SaveAsync(string url)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            url = url.Trim();

            if (!new Regex(YoutuberRegexp).Match(url).Success)
                throw new Exception("Link is not valid for youtube video");

            url = UrlFormatter.RemoveParametersFromUrl(url);

            var videoInfos = await _yotubeLinkerCore.GetDownloadUrlsAsync(url);
            var videoInfo  = videoInfos.OrderByDescending(x => x.AudioBitrate).First(info => info.VideoType == VideoType.Mp4);

            var videoFolderPath = _fileResolver.GetVideoFolderPath(StorageType.FileSystem);
            var fileName        = UrlFormatter.GetYoutubeVideoIdentifier(url);
            var fullPath        = Path.Combine(videoFolderPath, fileName + videoInfo.VideoExtension);

            if (File.Exists(fullPath))
                File.Delete(fullPath);
           
            await HttpHelper.DownloadAsync(videoInfo.DownloadUrl, fullPath);
            
            return new PCFile
            {
                Extension   = videoInfo.VideoExtension,
                Filename    = fileName,
                StorageType = StorageType.FileSystem
            };
        }        
    }
}