using Kw.Comic.Engine;
using Kw.Comic.Visit.Interceptors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kw.Comic.Visit
{
    public abstract class PageCursorBase<TChapterVisitor> : DataCursor<TChapterVisitor>
        where TChapterVisitor: ChapterVisitorBase
    {
        public IPageLoadInterceptor<TChapterVisitor> Interceptor { get; set; }

        public IComicSourceProvider SourceProvider { get; }

        public ChapterCursor ChapterCursor { get; }

        public PageCursorBase(ChapterCursor chapterCursor, 
            IComicSourceProvider sourceProvider,
            IReadOnlyList<TChapterVisitor> datas)
            : base(datas)
        {
            SourceProvider = sourceProvider;
            ChapterCursor = chapterCursor;
            Watch();
        }

        public PageCursorBase(ChapterCursor chapterCursor,
            IComicSourceProvider sourceProvider,
            IEnumerable<TChapterVisitor> datas)
            : base(datas)
        {
            this.SourceProvider = sourceProvider;
            ChapterCursor = chapterCursor;
            Watch();
        }
        private void Watch()
        {
            foreach (var item in Datas)
            {
                item.Loaded += OnItemLoaded;
                item.Loading += OnItemLoading;
            }
        }

        private void OnItemLoading(ChapterVisitorBase obj)
        {
            RaiseResourceLoading((TChapterVisitor)obj);
        }

        private void UnWatch()
        {
            foreach (var item in Datas)
            {
                item.Loaded -= OnItemLoaded;
                item.Loading -= OnItemLoading;
            }
        }
        private void OnItemLoaded(ChapterVisitorBase obj)
        {
            RaiseResourceLoaded((TChapterVisitor)obj);
        }

        public override async Task LoadIndexAsync(int index)
        {
            var val = this[index];
            if (!val.IsLoaded)
            {
                var interceptor = Interceptor;
                if (interceptor != null)
                {
                    await interceptor.LoadAsync(this, val);
                }
                else
                {
                    await val.LoadAsync();
                }
            }
        }
        ~PageCursorBase()
        {
            Dispose();
        }
        public override void Dispose()
        {
            UnWatch();
            foreach (var item in Datas)
            {
                item.Dispose();
            }
            base.Dispose();
        }
    }
}
