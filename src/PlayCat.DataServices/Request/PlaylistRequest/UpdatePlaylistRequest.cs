using System;

namespace PlayCat.DataService.Request
{
    public class UpdatePlaylistRequest : CreatePlaylistRequest
    {
        public Guid PlaylistId { get; set; }
    }
}
