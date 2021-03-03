using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Kw.Comic.Visit
{
    public class PageCursor<T> : PageCursorBase<T>
        where T : ChapterVisitorBase
    {
        public PageCursor(HttpClient httpclient, ChapterCursor chapterCursor, IReadOnlyList<T> datas)
            : base(httpclient,chapterCursor, datas)
        {
            Watch();
        }

        public PageCursor(HttpClient httpclient, ChapterCursor chapterCursor, IEnumerable<T> datas) 
            : base(httpclient,chapterCursor, datas)
        {
            Watch();
        }

        private void Watch()
        {
            foreach (var item in Datas)
            {
                item.Loaded += OnItemLoaded;
            }
        }
        private void UnWatch()
        {
            foreach (var item in Datas)
            {
                item.Loaded -= OnItemLoaded;
            }

        }
        public event Action<PageCursor<T>, ChapterVisitorBase> PageLoaded;
        private void OnItemLoaded(ChapterVisitorBase obj)
        {
            PageLoaded?.Invoke(this, obj);
        }
        public override void Dispose()
        {
            UnWatch();
            base.Dispose();
        }
    }
    public class PageCursor : PageCursorBase<ChapterVisitor>
    {
        public PageCursor(HttpClient httpclient, ChapterCursor chapterCursor, IReadOnlyList<ChapterVisitor> datas) 
            : base(httpclient,chapterCursor, datas)
        {
        }

        public PageCursor(HttpClient httpclient, ChapterCursor chapterCursor, IEnumerable<ChapterVisitor> datas)
            : base(httpclient,chapterCursor, datas)
        {
        }

    }
}
