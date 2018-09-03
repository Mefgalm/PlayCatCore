using PlayCat.ApiModels;

namespace PlayCat.DataServices.Response.AuthResponse
{
    public class SignUpInResult : BaseResult
    {
        public SignUpInResult() : base(new BaseResult())
        {
        }

        public SignUpInResult(BaseResult baseResult) : base(baseResult)
        {
        }

        public User User { get; set; }

        public AuthToken AuthToken { get; set; }
    }
}
