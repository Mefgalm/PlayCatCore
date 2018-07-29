using System.Threading.Tasks;

namespace PlayCat.Music
{
    public interface IVideoInfoGetter
    {
        Task<IUrlInfo> GetInfoAsync(string url);
    }
}
