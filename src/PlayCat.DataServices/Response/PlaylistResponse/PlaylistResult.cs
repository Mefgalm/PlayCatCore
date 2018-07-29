using PlayCat.ApiModel;

namespace PlayCat.DataService.Response.PlaylistResponse
{
    public class PlaylistResult : BaseResult
    {
        public PlaylistResult() : base(new BaseResult())
        {
        }

        public PlaylistResult(BaseResult baseResult) : base(baseResult)
        {
        }

        public Playlist Playlist { get; set; }
    }
}
