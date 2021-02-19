using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kw.Comic.Visit
{
    public class ChapterCursor : DataCursor<ComicVisitor>
    {

        public ChapterCursor(ImmutableArray<ComicVisitor> datas)
            : base(datas)
        {
        }

        public ChapterCursor(IEnumerable<ComicVisitor> datas)
            : base(datas)
        {
        }

        public override async Task LoadIndexAsync(int index)
        {
            var val = this[index];
            if (!val.IsLoaded)
            {
                await val.LoadAsync();
            }
        }
        public Task<PageCursor> MakePageCursorAsync(HttpClient httpClient)
        {
            return MakePageCursorAsync(Index, httpClient);
        }
        public async Task<PageCursor> MakePageCursorAsync(int i,HttpClient httpClient)
        {
            await LoadIndexAsync(i);
            return Datas[i].ChapterWithPage.Pages.MakePageCursor(httpClient);
        }
        public override void Dispose()
        {
            foreach (var item in Datas)
            {
                item.Dispose();
            }
            base.Dispose();
        }
    }
}
