using System;
using PlayCat.DataModels;
using PlayCat.DataServices.Request;

namespace PlayCat.DataServices.Mappers
{
    public static class UserMapper
    {
        public static class ToData
        {
            public static User FromRequest(SignUpRequest request, Action<User> overrides = null)
            {
                var dataUser = request == null ? null : new User
                {
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                };
                overrides?.Invoke(dataUser);

                return dataUser;
            }
        }

        public static class ToApi
        {
            public static ApiModels.User FromData(User dataUser)
            {
                return dataUser == null ? null : new ApiModels.User
                {
                    Email = dataUser.Email,
                    FirstName = dataUser.FirstName,
                    Id = dataUser.Id,
                    IsUploading = dataUser.IsUploadingAudio,
                    LastName = dataUser.LastName,
                    NickName = dataUser.NickName,
                    RegisterDate = dataUser.RegisterDate
                };
            }
        }
    }
}
