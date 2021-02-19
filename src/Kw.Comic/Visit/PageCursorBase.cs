using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kw.Comic.Visit
{
    public abstract class PageCursorBase<TChapterVisitor> : DataCursor<TChapterVisitor>
        where TChapterVisitor: ChapterVisitorBase
    {

        public HttpClient HttpClient { get; }

        public PageCursorBase(HttpClient httpclient, ImmutableArray<TChapterVisitor> datas)
            : base(datas)
        {
            HttpClient = httpclient;
        }

        public PageCursorBase(HttpClient httpclient, IEnumerable<TChapterVisitor> datas)
            : base(datas)
        {
            HttpClient = httpclient;
        }

        public event Action<PageCursorBase<TChapterVisitor>, TChapterVisitor> VisitorLoaded;

        public override async Task LoadIndexAsync(int index)
        {
            var val = this[index];
            if (!val.IsLoaded)
            {
                await val.LoadAsync();
                VisitorLoaded?.Invoke(this, val);
            }
        }
        ~PageCursorBase()
        {
            Dispose();
        }
        public override void Dispose()
        {
            foreach (var item in Datas)
            {
                item.Dispose();
            }
            base.Dispose();
            HttpClient?.Dispose();
        }
    }
}
