using PlayCat.Music;

namespace PlayCat.DataServices.Response.AudioResponse
{
    public class GetInfoResult : BaseResult
    {
        public GetInfoResult() : base(new BaseResult())
        {
        }

        public GetInfoResult(BaseResult baseResult) : base(baseResult)
        {
        }

        public IUrlInfo UrlInfo { get; set; }        
    }
}
