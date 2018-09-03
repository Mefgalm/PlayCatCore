using System;
using PlayCat.DataServices.Request;
using PlayCat.DataServices.Response;
using PlayCat.DataServices.Response.AudioResponse;

namespace PlayCat.DataServices
{
    public interface IAudioService : ISetDbContext
    {
        AudioResult GetAudios(Guid playlistId, int skip, int take);
        BaseResult RemoveFromPlaylist(Guid userId, AddRemovePlaylistRequest request);
        BaseResult AddToPlaylist(Guid userId, AddRemovePlaylistRequest request);
        AudioResult SearchAudios(string searchString, int skip, int take);
    }
}