using System;

namespace PlayCat.DataService.Request
{
    public class AddRemovePlaylistRequest
    {
        public Guid PlaylistId { get; set; }

        public Guid AudioId { get; set; }
    }
}
