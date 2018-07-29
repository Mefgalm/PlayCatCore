using PlayCat.DataService.DTO;

namespace PlayCat.DataService.Mappers
{
    public static class AudioMapper
    {
        public static class ToApi
        {
            private static string Host = "http://207.154.239.108/";
            
            public static ApiModel.Audio FromDTO(AudioDTO audioDTO)
            {
                return audioDTO == null ? null : new ApiModel.Audio
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

            public static ApiModel.Audio FromData(DataModel.Audio audio)
            {
                return audio == null ? null : new ApiModel.Audio
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
