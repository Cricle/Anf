using Anf.Easy.Visiting;
using System.Threading.Tasks;

namespace Anf.Easy.Test.Visiting
{
    internal class NullComicChapterManager<T> : IComicChapterManager<T>
    {
        public ChapterWithPage ChapterWithPage { get; set; }

        public Task<IComicVisitPage<T>> GetVisitPageAsync(int index)
        {
            return Task.FromResult<IComicVisitPage<T>>(null);
        }
    }
}
