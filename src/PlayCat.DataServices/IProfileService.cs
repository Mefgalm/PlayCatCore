using System;
using PlayCat.DataServices.Request;
using PlayCat.DataServices.Response.UserResponse;

namespace PlayCat.DataServices
{
    public interface IProfileService : ISetDbContext
    {
        GetUpdateProfileResult UpdateProfile(UpdateProfileRequest request);
        GetUpdateProfileResult GetProfile(Guid id);
    }
}
