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
    public class SkiaWpfComicWatcher : WpfComicWatcher<SkiaChapterVisitor>, IDisposable
    {
        public SkiaWpfComicWatcher(IServiceScope serviceScope, ComicEntity comic, IHttpClientFactory httpClientFactory, IComicSourceCondition condition, IComicSourceProvider comicSourceProvider) : base(serviceScope, comic, httpClientFactory, condition, comicSourceProvider)
        {
        }

        protected override ComicPageInfo<SkiaChapterVisitor> CreatePageInfo(SkiaChapterVisitor item)
        {
            return new SkiaComicPageInfo(item);
        }

        protected override async Task<PageCursorBase<SkiaChapterVisitor>> MakePageCursorAsync(int i, HttpClient httpClient)
        {
            await ChapterCursor.LoadIndexAsync(i);
            var pages = ChapterCursor.Datas[i].ChapterWithPage.Pages;
            var uwpc = new PageCursor<SkiaChapterVisitor>(httpClient,
                pages.Select(x => new SkiaChapterVisitor(x, httpClient)));
            return uwpc;
        }
    }
}
