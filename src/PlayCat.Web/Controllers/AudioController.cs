using System;
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
    public class AudioController : BaseController
    {
        private readonly IAudioService _audioService;
        private readonly IAuthService _authService;

        public AudioController(IAudioService audioService, IAuthService authService)
        {
            _audioService = audioService;
            _authService = authService;
        }

        [HttpGet("audios")]
        public AudioResult GetAudios(Guid playlistId, int skip, int take)
        {
            CheckTokenResult checkTokenResult = _authService.CheckToken(AccessToken);
            if (!checkTokenResult.Ok)
                return new AudioResult(checkTokenResult);

            return _audioService.GetAudios(playlistId, skip, take);
        }

        [HttpGet("search")]
        public AudioResult Search(string search, int skip, int take)
        {
            CheckTokenResult checkTokenResult = _authService.CheckToken(AccessToken);
            if (!checkTokenResult.Ok)
                return new AudioResult(checkTokenResult);

            return _audioService.SearchAudios(search, skip, take);
        }

        [HttpPut("addToPlaylist")]
        public BaseResult AddToPlaylist([FromBody] AddRemovePlaylistRequest request)
        {
            CheckTokenResult checkTokenResult = _authService.CheckToken(AccessToken);
            if (!checkTokenResult.Ok)
                return checkTokenResult;

            return _audioService.AddToPlaylist(checkTokenResult.AuthToken.UserId, request);
        }

        [HttpPut("removeFromPlaylist")]
        public BaseResult RemoveFromPlaylist([FromBody] AddRemovePlaylistRequest request)
        {
            CheckTokenResult checkTokenResult = _authService.CheckToken(AccessToken);
            if (!checkTokenResult.Ok)
                return checkTokenResult;

            return _audioService.RemoveFromPlaylist(checkTokenResult.AuthToken.UserId, request);
        }
    }
}