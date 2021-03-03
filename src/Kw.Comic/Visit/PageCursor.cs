using Kw.Comic.Engine;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Kw.Comic.Visit
{
    public class PageCursor<T> : PageCursorBase<T>
        where T : ChapterVisitorBase
    {
        public PageCursor(ChapterCursor chapterCursor,
            IComicSourceProvider sourceProvider,
            IReadOnlyList<T> datas)
            : base(chapterCursor, sourceProvider, datas)
        {
            Watch();
        }

        public PageCursor(ChapterCursor chapterCursor,
            IComicSourceProvider sourceProvider, 
            IEnumerable<T> datas) 
            : base(chapterCursor, sourceProvider, datas)
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
        public PageCursor(ChapterCursor chapterCursor,
            IComicSourceProvider sourceProvider,
            IReadOnlyList<ChapterVisitor> datas) 
            : base(chapterCursor,sourceProvider, datas)
        {
        }

        public PageCursor(ChapterCursor chapterCursor,
            IComicSourceProvider sourceProvider,
            IEnumerable<ChapterVisitor> datas)
            : base(chapterCursor,sourceProvider, datas)
        {
        }

    }
}
