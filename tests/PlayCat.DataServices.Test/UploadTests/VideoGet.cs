﻿using System;
using System.IO;
using Newtonsoft.Json;
using PlayCat.DataServices.Request;
using PlayCat.DataServices.Response.AudioResponse;
using PlayCat.DataServices.Test.Extensions;
using PlayCat.Music;
using Xunit;
using Xunit.Abstractions;

namespace PlayCat.DataServices.Test.UploadTests
{
    public class VideoGet : BaseTest
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("123")]
        [InlineData("htt://ww.youtube.com/watch?v=Qa4u4D32x4U")]
        [InlineData("htp://www.youtube.com/watch?v=Qa4u4D32x4U")]
        [InlineData("http://ww.youtube.com/watch?v=Qa4u4D32x4U")]
        [InlineData("https://ww.youtube.com/watch?v=Qa4u4D32x4U")]
        [InlineData("https://youtu.be/Qa4u4D32x4U&param=2")]
        public void IsErrorOnInvalidUrl(string url)
        {
            var uploadService = _server.Host.Services.GetService(typeof(IUploadService)) as IUploadService;

            var request = new UrlRequest
            {
                Url = url
            };

            GetInfoResult result = uploadService.GetInfoAsync(request).Result;

            CheckIfFail(result);

            Assert.Equal("Model is not valid", result.Info);
            Assert.False(result.ShowInfo);
            Assert.NotNull(result.Errors);
            Assert.Equal(result.Errors.Count, 1);
        }

        [Fact]
        public void IsWrongUrlId()
        {
            var uploadService = _server.Host.Services.GetService(typeof(IUploadService)) as IUploadService;

            var request = new UrlRequest
            {
                Url = "https://www.youtube.com/watch?v=11111111"
            };

            GetInfoResult result = uploadService.GetInfoAsync(request).Result;

            CheckIfFail(result);

            Assert.True(result.Info.Length > 0);
            Assert.Null(result.Errors);
        }

        [Fact]
        public void IsVideoSizeToLarge()
        {
            var uploadService = _server.Host.Services.GetService(typeof(IUploadService)) as IUploadService;

            var request = new UrlRequest
            {
                Url = "https://www.youtube.com/watch?v=2mI0nEgdgsA"
            };

            GetInfoResult result = uploadService.GetInfoAsync(request).Result;

            CheckIfFail(result);

            Assert.Equal("Maximim video size is 25 MB", result.Info);
            Assert.True(result.ShowInfo);
            Assert.Null(result.Errors);
        }

        [Fact]
        public void ShouldFailOnAlreadyUpload()
        {
            SqlLiteDatabaseTest(options =>
            {
                var uploadService = _server.Host.Services.GetService(typeof(IUploadService)) as IUploadService;
                var fileResolver = _server.Host.Services.GetService(typeof(IFileResolver)) as IFileResolver;

                using (var context = new PlayCatDbContext(options))
                {
                    uploadService.SetDbContext(context);

                    string youtubeUrl = "https://www.youtube.com/watch?v=kK4JSz7uKzI";

                    var request = new UrlRequest
                    {
                        Url = youtubeUrl
                    };

                    GetInfoResult result = uploadService.GetInfoAsync(request).Result;

                    CheckIfSuccess(result);

                    Guid userId = GetUserId(context);
                    context.CreatePlaylist(true, userId, null, 0);
                    context.SaveChanges();

                    Assert.NotNull(result.UrlInfo);
                    Assert.Equal("Jacoo", result.UrlInfo.Artist);
                    Assert.Equal("Revelation (Original)", result.UrlInfo.Song);

                    var uploadResult = uploadService.UploadAudioAsync(userId, new UploadAudioRequest
                    {
                        Artist = "Jacoo",
                        Song = "Revelation (Original)",
                        Url = youtubeUrl
                    }).Result;

                    CheckIfSuccess(uploadResult);

                    string audioFilePath = fileResolver.GetAudioFolderPath(StorageType.FileSystem);
                    File.Delete(Path.Combine(audioFilePath, "yPYZpwSpKmA.mp3"));

                    GetInfoResult checkAgainResult = uploadService.GetInfoAsync(request).Result;

                    CheckIfFail(checkAgainResult);
                    Assert.Equal("Video already uploaded", checkAgainResult.Info);
                }
            });
        }

        [Theory]
        [InlineData("https://www.youtube.com/watch?v=80AlC3LaPqQ")]
        [InlineData("http://www.youtube.com/watch?v=80AlC3LaPqQ")]
        [InlineData("www.youtube.com/watch?v=80AlC3LaPqQ")]
        [InlineData("youtube.com/watch?v=80AlC3LaPqQ")]
        [InlineData("http://www.youtube.com/watch?v=80AlC3LaPqQ&t=33")]
        [InlineData("https://www.youtube.com/watch?v=80AlC3LaPqQ&t=33")]
        [InlineData("https://www.youtube.com/watch?v=80AlC3LaPqQ&t=33&featured=youtu.be")]
        [InlineData("https://youtu.be/80AlC3LaPqQ")]
        [InlineData("http://youtu.be/80AlC3LaPqQ")]
        public void IsValidUrl(string url)
        {
            SqlLiteDatabaseTest(options =>
            {
                var uploadService = _server.Host.Services.GetService(typeof(IUploadService)) as IUploadService;

                using (var context = new PlayCatDbContext(options))
                {
                    uploadService.SetDbContext(context);

                    var request = new UrlRequest
                    {
                        Url = url
                    };

                    GetInfoResult result = uploadService.GetInfoAsync(request).Result;
                    
                    //Debug.WriteLine(JsonConvert.SerializeObject(result));

                    _output.WriteLine(JsonConvert.SerializeObject(result));
                    
                    CheckIfSuccess(result);

                    Assert.NotNull(result.UrlInfo);
                    Assert.Equal("Flume", result.UrlInfo.Artist);
                    Assert.Equal("Say It (feat. Tove Lo) (Illenium Remix)", result.UrlInfo.Song);
                    Assert.Equal(8023661, result.UrlInfo.ContentLength);
                }
            });            
        }

        public VideoGet(ITestOutputHelper output) : base(output)
        {
        }
    }
}
