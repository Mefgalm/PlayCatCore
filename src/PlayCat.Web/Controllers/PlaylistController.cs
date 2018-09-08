using System;
using Microsoft.AspNetCore.Mvc;
using PlayCat.DataServices;
using PlayCat.DataServices.Request;
using PlayCat.DataServices.Response;
using PlayCat.DataServices.Response.AuthResponse;
using PlayCat.DataServices.Response.PlaylistResponse;

namespace PlayCat.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class PlaylistController : BaseController
    {
        private readonly IPlaylistService _playlistService;
        private readonly IAuthService _authService;

        public PlaylistController(IPlaylistService playlistService, IAuthService authService)
        {
            _playlistService = playlistService;
            _authService = authService;
        }

        [HttpDelete("delete/{playlistId}")]
        public BaseResult DeletePlaylist(Guid playlistId)
        {
            CheckTokenResult checkTokenResult = _authService.CheckToken(AccessToken);
            if (!checkTokenResult.Ok)
                return checkTokenResult;

            return _playlistService.DeletePlaylist(checkTokenResult.AuthToken.UserId, playlistId);
        }

        [HttpPut("update")]
        public PlaylistResult UpdatePlaylist([FromBody] UpdatePlaylistRequest request)
        {
            CheckTokenResult checkTokenResult = _authService.CheckToken(AccessToken);
            if (!checkTokenResult.Ok)
                return new PlaylistResult(checkTokenResult);

            return _playlistService.UpdatePlaylist(checkTokenResult.AuthToken.UserId, request);
        }

        [HttpPost("create")]
        public PlaylistResult CreatePlaylist([FromBody] CreatePlaylistRequest request)
        {
            CheckTokenResult checkTokenResult = _authService.CheckToken(AccessToken);
            if (!checkTokenResult.Ok)
                return new PlaylistResult(checkTokenResult);

            return _playlistService.CreatePlaylist(checkTokenResult.AuthToken.UserId, request);
        }

        [HttpGet("allUserPlaylists")]
        public AllUserPlaylistsResult AllUserPlaylistsResult()
        {
            CheckTokenResult checkTokenResult = _authService.CheckToken(AccessToken);
            if (!checkTokenResult.Ok)
                return new AllUserPlaylistsResult(checkTokenResult);

            return _playlistService.AllUserPlaylists(checkTokenResult.AuthToken.UserId);
        }

        [HttpGet("userPlaylists")]
        public UserPlaylistsResult GetUserPlaylists(Guid? playlistId, int skip = 0, int take = 50)
        {
            CheckTokenResult checkTokenResult = _authService.CheckToken(AccessToken);
            if (!checkTokenResult.Ok)
                return new UserPlaylistsResult(checkTokenResult);

            return _playlistService.GetUserPlaylists(checkTokenResult.AuthToken.UserId, playlistId, skip, take);
        }
    }
}
