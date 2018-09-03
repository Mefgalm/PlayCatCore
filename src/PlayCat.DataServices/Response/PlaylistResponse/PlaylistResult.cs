using PlayCat.ApiModels;

namespace PlayCat.DataServices.Response.PlaylistResponse
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
