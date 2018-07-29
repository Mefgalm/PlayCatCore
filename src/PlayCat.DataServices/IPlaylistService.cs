using System;
using PlayCat.DataService.Request;
using PlayCat.DataService.Response;
using PlayCat.DataService.Response.PlaylistResponse;

namespace PlayCat.DataService
{
    public interface IPlaylistService : ISetDbContext
    {
        PlaylistResult CreatePlaylist(Guid userId, CreatePlaylistRequest request);
        BaseResult DeletePlaylist(Guid userId, Guid playlistId);
        UserPlaylistsResult GetUserPlaylists(Guid userId, Guid? playlistId, int skip, int take);
        PlaylistResult UpdatePlaylist(Guid userId, UpdatePlaylistRequest request);
    }
}