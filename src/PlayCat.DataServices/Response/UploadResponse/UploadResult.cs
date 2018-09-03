using PlayCat.ApiModels;

namespace PlayCat.DataServices.Response.UploadResponse
{
    public class UploadResult : BaseResult
    {
        public UploadResult() : base(new BaseResult())
        {
        }

        public UploadResult(BaseResult baseResult) : base(baseResult)
        {
        }

        public Audio Audio { get; set; }
    }
}
