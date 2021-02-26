using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net.Http;

namespace Kw.Comic.Visit
{
    public class PageCursor<T> : PageCursorBase<T>
        where T : ChapterVisitorBase
    {
        public PageCursor(HttpClient httpclient, ImmutableArray<T> datas) : base(httpclient, datas)
        {
        }

        public PageCursor(HttpClient httpclient, IEnumerable<T> datas) : base(httpclient, datas)
        {
        }
    }
    public class PageCursor : PageCursorBase<ChapterVisitor>
    {
        public PageCursor(HttpClient httpclient, ImmutableArray<ChapterVisitor> datas) : base(httpclient, datas)
        {
        }

        public PageCursor(HttpClient httpclient, IEnumerable<ChapterVisitor> datas) : base(httpclient, datas)
        {
        }
    }
}
