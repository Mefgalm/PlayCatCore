using System.Collections.Generic;
using System.Linq;
using PlayCat.ApiModels;

namespace PlayCat.DataServices.Response.PlaylistResponse
{
    public class AllUserPlaylistsResult : BaseResult
    {
        public AllUserPlaylistsResult() : base(new BaseResult())
        {
        }

        public AllUserPlaylistsResult(BaseResult baseResult) : base(baseResult)
        {
        }

        public IEnumerable<SimplePlaylist> Playlists { get; set; } = Enumerable.Empty<SimplePlaylist>();
    }
}