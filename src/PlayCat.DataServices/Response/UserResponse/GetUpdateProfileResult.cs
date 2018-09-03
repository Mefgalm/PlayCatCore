using PlayCat.ApiModels;

namespace PlayCat.DataServices.Response.UserResponse
{
    public class GetUpdateProfileResult : BaseResult
    {
        public GetUpdateProfileResult() : base(new BaseResult())
        {
        }

        public GetUpdateProfileResult(BaseResult baseResult) : base(baseResult)
        {
        }

        public User User { get; set; }
    }
}
