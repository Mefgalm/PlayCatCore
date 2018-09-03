using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PlayCat.DataServices;
using PlayCat.DataServices.Request;
using PlayCat.DataServices.Response;
using PlayCat.DataServices.Response.AudioResponse;
using PlayCat.DataServices.Response.AuthResponse;

namespace PlayCat.Web.Controllers
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
