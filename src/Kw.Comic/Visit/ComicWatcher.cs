using Kw.Comic.Engine;
using System.Collections;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Visit
{
    public class ComicWatcher : ComicWatcherBase<ChapterVisitor>
    {
        public ComicWatcher(ComicEntity comic,
            IComicSourceCondition condition,
            IComicSourceProvider comicSourceProvider)
            : base(comic, condition, comicSourceProvider)
        {
        }

        protected override async Task<PageCursorBase<ChapterVisitor>> MakePageCursorAsync(int i)
        {
            var val = await ChapterCursor.MakePageCursorAsync(i);
            return val;
        }
    }
}
