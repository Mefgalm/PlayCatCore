using PlayCat.ApiModel;

namespace PlayCat.DataService.Response.UploadResponse
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
