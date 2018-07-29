using System.Threading.Tasks;

namespace PlayCat.Music
{
    public interface ISaveVideo
    {        
        Task<IFile> SaveAsync(string url);
    }
}
