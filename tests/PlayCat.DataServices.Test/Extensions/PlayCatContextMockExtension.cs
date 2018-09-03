using System;
using PlayCat.DataModels;

namespace PlayCat.DataServices.Test.Extensions
{
    internal static class PlayCatContextMockExtension
    {
        public static User CreateUser(this PlayCatDbContext context, string email, string firstname, string lastname, string nickName, string password)
        {
            var salt = new Crypto().GenerateSalt();
            var passwordHah = new Crypto().HashPassword(salt, password);

            return context.Users.Add(new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                NickName = nickName,
                FirstName = firstname,
                LastName = lastname,
                PasswordHash = passwordHah,
                PasswordSalt = salt,
                RegisterDate = DateTime.Now,
            }).Entity;
        }

        public static AuthToken CreateToken(this PlayCatDbContext context, DateTime dateExpired, bool isActive, Guid userId)
        {
            return context.AuthTokens.Add(new AuthToken
            {
                Id = Guid.NewGuid(),
                DateExpired = dateExpired,
                IsActive = isActive,
                UserId = userId
            }).Entity;
        }

        public static Playlist CreatePlaylist(this PlayCatDbContext context, bool isGeneral, Guid ownerId, string title, int orderValue)
        {
            return context.Playlists.Add(new Playlist
            {
                Id = Guid.NewGuid(),
                IsGeneral = isGeneral,
                OwnerId = ownerId,
                Title = title,
                OrderValue = orderValue
            }).Entity;
        }

        public static Audio CreateAudio(this PlayCatDbContext context, DateTime created, string accessUrl, string artist, string song, string extension, string filename, string videoId, Guid? uploaderId)
        {            
            return context.Audios.Add(new Audio
            {
                Id = Guid.NewGuid(),
                AccessUrl = accessUrl,
                DateCreated = created,
                Artist = artist,
                Song = song,
                Extension = extension,
                FileName = filename,
                UniqueIdentifier = videoId,
                UploaderId = uploaderId
            }).Entity;
        }

        public static AudioPlaylist CreateAudioPlaylist(this PlayCatDbContext context, DateTime dateAdded, Guid audioId, Guid playlistId, int order)
        {
            return context.AudioPlaylists.Add(new AudioPlaylist
            {
                AudioId = audioId,
                DateCreated = dateAdded,
                PlaylistId = playlistId,
                Order = order
            }).Entity;
        }
    }
}
