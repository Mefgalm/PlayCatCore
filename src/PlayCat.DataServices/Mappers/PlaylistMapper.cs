using System.Linq;
using PlayCat.ApiModels;
using PlayCat.DataServices.DTO;

namespace PlayCat.DataServices.Mappers
{
    public static class PlaylistMapper
    {
        public static class ToApi
        {
            public static Playlist FromDTO(PlaylistDTO playlistDTO)
            {
                return playlistDTO == null ? null : new Playlist
                {
                    Id = playlistDTO.Id,
                    Title = playlistDTO.Title,
                    IsGeneral = playlistDTO.IsGeneral,
                    Owner = UserMapper.ToApi.FromData(playlistDTO.Owner),
                    Audios = playlistDTO.Audios.Select(x => AudioMapper.ToApi.FromDTO(x))
                };
            }

            public static Playlist FromData(DataModels.Playlist playlist)
            {
                return playlist == null ? null : new Playlist
                {
                    Id = playlist.Id,
                    IsGeneral = playlist.IsGeneral,
                    Title = playlist.Title,
                    Owner = UserMapper.ToApi.FromData(playlist.Owner)
                };
            }
        }
    }
}
