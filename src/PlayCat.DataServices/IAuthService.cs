using PlayCat.DataServices.Request;
using PlayCat.DataServices.Response.AuthResponse;

namespace PlayCat.DataServices
{
    public interface IAuthService : ISetDbContext
    {
        CheckTokenResult CheckToken(string token);
        SignUpInResult SignIn(SignInRequest request);
        SignUpInResult SignUp(SignUpRequest request);
    }
}