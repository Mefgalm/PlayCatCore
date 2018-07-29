using System.Collections.Generic;
using System.Linq;
using PlayCat.ApiModel;

namespace PlayCat.DataService.Response.PlaylistResponse
{
    public class UserPlaylistsResult : BaseResult
    {
        public UserPlaylistsResult() : base(new BaseResult())
        {
        }

        public UserPlaylistsResult(BaseResult baseResult) : base(baseResult)
        {
        }

        public IEnumerable<Playlist> Playlists { get; set; } = Enumerable.Empty<Playlist>();
    }
}
