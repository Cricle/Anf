using Kw.Comic.Engine;
using Kw.Comic.Visit;
using Kw.Comic.Wpf.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Wpf.Managers
{
    public class SoftwareWpfComicWatcher : WpfComicWatcher<SoftwareChapterVisitor>, IDisposable
    {
        public SoftwareWpfComicWatcher(IServiceScope serviceScope, ComicEntity comic, 
            IComicSourceCondition condition, IComicSourceProvider comicSourceProvider) 
            : base(serviceScope, comic, condition, comicSourceProvider)
        {
        }

        protected override WpfComicPageInfo<SoftwareChapterVisitor> CreatePageInfo(SoftwareChapterVisitor item)
        {
            return new SoftwareComicPageInfo(item);
        }

        protected override async Task<PageCursorBase<SoftwareChapterVisitor>> MakePageCursorAsync(int i)
        {
            await ChapterCursor.LoadIndexAsync(i);
            var pages = ChapterCursor.Datas[i].ChapterWithPage.Pages;
            var uwpc = new PageCursor<SoftwareChapterVisitor>(ChapterCursor,ComicSourceProvider,
                pages.Select(x => new SoftwareChapterVisitor(x,ComicSourceProvider)));
            return uwpc;
        }
    }
}
