using System;
using System.Threading.Tasks;
using PlayCat.DataServices.Request;
using PlayCat.DataServices.Response.AudioResponse;
using PlayCat.DataServices.Response.UploadResponse;

namespace PlayCat.DataServices
{
    public interface IUploadService : ISetDbContext
    {
        Task<GetInfoResult> GetInfoAsync(UrlRequest request);
        Task<UploadResult> UploadAudioAsync(Guid userId, UploadAudioRequest request);
    }
}