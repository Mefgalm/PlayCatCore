using PlayCat.DataService.Request;
using PlayCat.DataService.Response;
using System;
using PlayCat.DataService.Response.UserResponse;

namespace PlayCat.DataService
{
    public interface IProfileService : ISetDbContext
    {
        GetUpdateProfileResult UpdateProfile(UpdateProfileRequest request);
        GetUpdateProfileResult GetProfile(Guid id);
    }
}
