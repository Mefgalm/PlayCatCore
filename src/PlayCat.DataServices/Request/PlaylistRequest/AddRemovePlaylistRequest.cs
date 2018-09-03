using System;

namespace PlayCat.DataServices.Request
{
    public class AddRemovePlaylistRequest
    {
        public Guid PlaylistId { get; set; }

        public Guid AudioId { get; set; }
    }
}
