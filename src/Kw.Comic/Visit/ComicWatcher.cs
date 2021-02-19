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
            IHttpClientFactory httpClientFactory,
            IComicSourceCondition condition,
            IComicSourceProvider comicSourceProvider)
            : base(comic, httpClientFactory, condition, comicSourceProvider)
        {
        }

        protected override async Task<PageCursorBase<ChapterVisitor>> MakePageCursorAsync(int i,HttpClient httpClient)
        {
            var val = await ChapterCursor.MakePageCursorAsync(i,httpClient);
            return val;
        }
        protected override async Task<PageCursorBase<ChapterVisitor>> MakePageCursorAsync(HttpClient httpClient)
        {
            var val = await ChapterCursor.MakePageCursorAsync(ChapterCursor.Index, httpClient);
            return val;
        }
    }
}
