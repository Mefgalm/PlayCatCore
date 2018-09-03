using System;
using System.Collections.Generic;
using PlayCat.DataModels;

namespace PlayCat.DataServices.DTO
{
    public class AudioDTO
    {
        public string Artist { get; set; }

        public string Song { get; set; }

        public Guid Id { get; set; }

        public double Duration { get; set; }

        public string AccessUrl { get; set; }

        public DateTime DateAdded { get; set; }

        public User Uploader { get; set; }
    }

    public class PlaylistDTO
    {
        public string Title { get; set; }

        public Guid Id { get; set; }

        public bool IsGeneral { get; set; }

        public User Owner { get; set; }

        public IEnumerable<AudioDTO> Audios { get; set; }
    }
}
