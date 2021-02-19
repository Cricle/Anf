using System.Threading.Tasks;

namespace Kw.Comic.Visit
{
    public interface IComicSaver
    {
        Task ResolvedChapterAsync(ResolvedChapterContext context);

        Task SaveAsync(ComicSaveContext context);
    }
}
