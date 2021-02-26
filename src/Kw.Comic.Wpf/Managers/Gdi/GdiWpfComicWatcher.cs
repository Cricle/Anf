using Kw.Comic.Engine;
using Kw.Comic.Visit;
using Kw.Comic.Wpf.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kw.Comic.Wpf.Managers
{
    public class GdiWpfComicWatcher : WpfComicWatcher<GdiChapterVisitor>, IDisposable
    {
        public GdiWpfComicWatcher(IServiceScope serviceScope, ComicEntity comic, IHttpClientFactory httpClientFactory, IComicSourceCondition condition, IComicSourceProvider comicSourceProvider) : base(serviceScope, comic, httpClientFactory, condition, comicSourceProvider)
        {
        }

        protected override WpfComicPageInfo<GdiChapterVisitor> CreatePageInfo(GdiChapterVisitor item)
        {
            return new GdiComicPageInfo(item);
        }

        protected override async Task<PageCursorBase<GdiChapterVisitor>> MakePageCursorAsync(int i, HttpClient httpClient)
        {
            await ChapterCursor.LoadIndexAsync(i);
            var pages = ChapterCursor.Datas[i].ChapterWithPage.Pages;
            var uwpc = new PageCursor<GdiChapterVisitor>(httpClient,
                pages.Select(x => new GdiChapterVisitor(x, httpClient)));
            return uwpc;
        }
    }
}
