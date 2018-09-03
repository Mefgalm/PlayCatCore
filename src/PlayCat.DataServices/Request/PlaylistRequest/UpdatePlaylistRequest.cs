using System;

namespace PlayCat.DataServices.Request
{
    public class UpdatePlaylistRequest : CreatePlaylistRequest
    {
        public Guid PlaylistId { get; set; }
    }
}
