using System;
using PlayCat.DataServices.Request;
using PlayCat.DataServices.Response;
using PlayCat.DataServices.Response.PlaylistResponse;

namespace PlayCat.DataServices
{
    public interface IPlaylistService : ISetDbContext
    {
        PlaylistResult CreatePlaylist(Guid userId, CreatePlaylistRequest request);
        BaseResult DeletePlaylist(Guid userId, Guid playlistId);
        UserPlaylistsResult GetUserPlaylists(Guid userId, Guid? playlistId, int skip, int take);
        PlaylistResult UpdatePlaylist(Guid userId, UpdatePlaylistRequest request);
        AllUserPlaylistsResult AllUserPlaylists(Guid userId);
    }
}