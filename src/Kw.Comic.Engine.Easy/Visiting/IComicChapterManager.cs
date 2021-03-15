using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy.Visiting
{
    public interface IComicChapterManager
    {
        ChapterWithPage ChapterWithPage { get; }

        Task<IComicVisitPage> GetVisitPageAsync(int index);
    }
}
