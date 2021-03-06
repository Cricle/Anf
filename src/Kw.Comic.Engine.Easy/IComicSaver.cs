using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy
{
    public interface IComicSaver
    {
        Task SaveAsync(ComicDownloadContext context);
    }
}
