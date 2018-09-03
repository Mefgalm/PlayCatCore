using Microsoft.Extensions.DependencyInjection;
using PlayCat.Music;
using PlayCat.Music.Youtube;

namespace PlayCat.DataServices
{
    public static class ServiceProvider
    {
        public static void RegisterServices(IServiceCollection service)
        {
            service.AddScoped<ISaveVideo, YoutubeSaveVideo>();
            service.AddScoped<IExtractAudio, FFmpegExtractAudio>();
            service.AddScoped<IUploadAudio, UploadAudio>();
            service.AddScoped<IVideoInfoGetter, YoutubeVideoInfoGetter>();
            service.AddScoped<IFileResolver, FileResolver>();
            service.AddScoped<IAuthService, AuthService>();
            service.AddScoped<IModelValidationService, ModelValidationService>();
            service.AddScoped<IProfileService, ProfileService>();
            service.AddScoped<ICrypto, Crypto>();
            service.AddScoped<IYotubeLinkerCore, YoutubeLinkerCore>();

            service.AddScoped<IAudioService, AudioService>();
            service.AddScoped<IUploadService, UploadService>();
            service.AddScoped<IPlaylistService, PlaylistService>();
        }
    }
}
