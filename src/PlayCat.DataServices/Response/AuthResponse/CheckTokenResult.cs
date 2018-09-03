using PlayCat.DataModels;

namespace PlayCat.DataServices.Response.AuthResponse
{
    public class CheckTokenResult : BaseResult
    {
        public AuthToken AuthToken { get; set; }
    }
}
