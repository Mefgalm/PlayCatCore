using Microsoft.AspNetCore.Mvc;
using PlayCat.DataServices;
using PlayCat.DataServices.Request;
using PlayCat.DataServices.Response.AuthResponse;

namespace PlayCat.Web.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("signUp")]
        public SignUpInResult SignUp([FromBody] SignUpRequest request)
        { 
            return _authService.SignUp(request);
        }

        [HttpPost("signIn")]
        public SignUpInResult SignIn([FromBody] SignInRequest request)
        {
            return _authService.SignIn(request);
        }
    }
}
