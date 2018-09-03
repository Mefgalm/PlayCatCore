using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlayCat.DataModels
{
    public class AudioPlaylist
    {
        public Guid AudioId { get; set; }

        [ForeignKey(nameof(AudioId))]
        public virtual Audio Audio { get; set; }

        public Guid PlaylistId { get; set; }

        [ForeignKey(nameof(PlaylistId))]
        public virtual Playlist Playlist { get; set; }

        public DateTime DateCreated { get; set; }

        public int Order { get; set; }
    }
}
