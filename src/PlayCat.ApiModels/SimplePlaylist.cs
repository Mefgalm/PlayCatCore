using System;

namespace PlayCat.ApiModels
{
    public class SimplePlaylist
    {
        public string Title { get; set; }

        public bool IsGeneral { get; set; }

        public Guid Id { get; set; }        

        public User Owner { get; set; }
    }
}