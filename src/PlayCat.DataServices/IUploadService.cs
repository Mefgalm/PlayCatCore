using System;
using PlayCat.DataService.Request;
using PlayCat.DataService.Response;
using PlayCat.DataService.Response.UploadResponse;
using System.Threading.Tasks;
using PlayCat.DataService.Response.AudioResponse;

namespace PlayCat.DataService
{
    public interface IUploadService : ISetDbContext
    {
        Task<GetInfoResult> GetInfoAsync(UrlRequest request);
        Task<UploadResult> UploadAudioAsync(Guid userId, UploadAudioRequest request);
    }
}