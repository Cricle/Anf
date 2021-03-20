using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy.Visiting
{
    public interface IComicChapterManager<TResource>
    {
        ChapterWithPage ChapterWithPage { get; }

        Task<IComicVisitPage<TResource>> GetVisitPageAsync(int index);
    }
}
