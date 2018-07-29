using Microsoft.AspNetCore.Mvc;
using PlayCat.DataService;
using PlayCat.DataService.Request;
using PlayCat.DataService.Response;
using System.Threading.Tasks;
using PlayCat.DataService.Response.AudioResponse;
using PlayCat.DataService.Response.AuthResponse;

namespace PlayCat.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class UploadController : BaseController
    {
        private readonly IUploadService _uploadService;
        private readonly IAuthService _authService;

        public UploadController(IUploadService uploadService, IAuthService authService)
        {
            _uploadService = uploadService;
            _authService = authService;
        }

        [HttpPost("videoInfo")]
        public async Task<GetInfoResult> GetUrlInfoAsync([FromBody] UrlRequest request)
        {
            CheckTokenResult checkTokenResult = _authService.CheckToken(AccessToken);
            if (!checkTokenResult.Ok)
                return new GetInfoResult(checkTokenResult);

            return await _uploadService.GetInfoAsync(request);
        }

        [HttpPost("uploadAudio")]
        public async Task<BaseResult> UploadAudio([FromBody] UploadAudioRequest request)
        {
            CheckTokenResult checkTokenResult = _authService.CheckToken(AccessToken);
            if (!checkTokenResult.Ok)
                return checkTokenResult;

            return await _uploadService.UploadAudioAsync(checkTokenResult.AuthToken.UserId, request);
        }
    }
}
