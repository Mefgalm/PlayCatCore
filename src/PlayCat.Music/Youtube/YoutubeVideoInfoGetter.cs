using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PlayCat.Helpers;
using YotubeLinkerCore;

namespace PlayCat.Music.Youtube
{
    public class YoutubeVideoInfoGetter : IVideoInfoGetter
    {
        private const string YoutuberRegexp = @"^(http(s)??\:\/\/)?(www\.)?(youtube\.com\/watch\?v=[\.A-Za-z0-9_\?=&-]+|youtu\.be\/[A-Za-z0-9_-]+)$";

        private readonly IYotubeLinkerCore _yotubeLinkerCore;

        public YoutubeVideoInfoGetter(IYotubeLinkerCore yotubeLinkerCore)
        {
            _yotubeLinkerCore = yotubeLinkerCore;
        }
        
        public async Task<IUrlInfo> GetInfoAsync(string url)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            url = url.Trim();

            if (!new Regex(YoutuberRegexp).Match(url).Success)
                throw new Exception("Link is not valid for youtube video");

            url = UrlFormatter.RemoveParametersFromUrl(url);

            var videoInfos = await _yotubeLinkerCore.GetDownloadUrlsAsync(url);
            var videoInfo = videoInfos.First(info => info.VideoType == VideoType.Mp4 && info.Resolution == 360);

            var headers = HttpRequester.GetHeaders(videoInfo.DownloadUrl);
            var artistAndSong = GetArtistAndSongName(videoInfo.Title);

            return new UrlInfo
            {
               Artist = artistAndSong.Artist,
               Song = artistAndSong.Song,
               ContentLength = headers.ContentLenght,
               VideoId = UrlFormatter.GetYoutubeVideoIdentifier(url)
            };
        }

        private (string Artist, string Song) GetArtistAndSongName(string title)
        {
            if (title == null)
                return (string.Empty, string.Empty);

            var artistAndSong = title.Split('-');
            if (artistAndSong.Length >= 2)
            {
                return (artistAndSong[0].Trim(), artistAndSong[1].Trim());
            }

            return (title, string.Empty);
        }
    }
}
