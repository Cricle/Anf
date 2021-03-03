using Kw.Comic.Engine;
using Kw.Comic.Visit.Interceptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kw.Comic.Visit
{
    public class ChapterCursor : DataCursor<ComicVisitor>
    {

        public ChapterCursor(ComicEntity comic, IComicSourceProvider sourceProvider, IReadOnlyList<ComicVisitor> datas)
            : base(datas)
        {
            this.SourceProvider = sourceProvider;
            Comic = comic;
            Watch();
        }

        public ChapterCursor(ComicEntity comic, IComicSourceProvider sourceProvider, IEnumerable<ComicVisitor> datas)
            : base(datas)
        {
            this.SourceProvider = sourceProvider;
            Comic = comic;
            Watch();
        }
        private void Watch()
        {
            foreach (var item in this.Datas)
            {
                item.Loaded += OnItemLoaded;
            }
        }
        private void UnWatch()
        {
            foreach (var item in this.Datas)
            {
                item.Loaded -= OnItemLoaded;
            }
        }

        private void OnItemLoaded(ComicVisitor arg1, ChapterWithPage arg2)
        {
            ComicVisitorLoaded?.Invoke(this, arg1, arg2);
        }

        public ComicEntity Comic { get; }

        public IComicSourceProvider SourceProvider { get; }

        public IChapterLoadInterceptor Interceptor { get; set; }

        public event Action<ChapterCursor, ComicVisitor, ChapterWithPage> ComicVisitorLoaded;

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
        public Task<PageCursor> MakePageCursorAsync()
        {
            return MakePageCursorAsync(Index);
        }
        public async Task<PageCursor> MakePageCursorAsync(int i)
        {
            await LoadIndexAsync(i);
            var pages= Datas[i].ChapterWithPage.Pages.MakePageCursor(this);
            return pages;
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
