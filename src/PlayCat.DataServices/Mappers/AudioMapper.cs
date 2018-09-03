using PlayCat.ApiModels;
using PlayCat.DataServices.DTO;

namespace PlayCat.DataServices.Mappers
{
    public static class AudioMapper
    {
        public static class ToApi
        {
            //private static string Host = "http://207.154.239.108/";
            private static string Host = "http://localhost:5000";
            
            public static Audio FromDTO(AudioDTO audioDTO)
            {
                return audioDTO == null ? null : new Audio
                {
                    //TODO use iunterfaces
                    AccessUrl = Host + audioDTO.AccessUrl,
                    Artist = audioDTO.Artist,
                    DateAdded = audioDTO.DateAdded,
                    Id = audioDTO.Id,
                    Duration = audioDTO.Duration,
                    Song = audioDTO.Song,
                    Uploader = UserMapper.ToApi.FromData(audioDTO.Uploader)                    
                };
            }

            public static Audio FromData(DataModels.Audio audio)
            {
                return audio == null ? null : new Audio
                {
                    AccessUrl = Host + audio.AccessUrl,
                    Artist = audio.Artist,
                    Id = audio.Id,
                    Duration = audio.Duration,
                    DateAdded = audio.DateCreated,
                    Song = audio.Song,
                    Uploader = UserMapper.ToApi.FromData(audio.Uploader)
                };
            }
        }
    }
}
