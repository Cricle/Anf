using Microsoft.IO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy.Visiting
{
    internal class ComicVisiting : IComicVisiting, IDisposable
    {
        private string address;
        private ComicEntity entity;
        private IResourceFactoryCreator resourceFactoryCreator;
        private IComicSourceProvider sourceProvider;
        private IResourceFactory resourceFactory;

        public IComicHost Host { get; }
        public IComicSourceProvider SourceProvider => sourceProvider;

        public string Address => address;
        public ComicEntity Entity => entity;

        public IResourceFactory ResourceFactory => resourceFactory;

        public IComicVisitingInterceptor VisitingInterceptor { get; set; }

        public IResourceFactoryCreator ResourceFactoryCreator
        {
            get => resourceFactoryCreator;
            set => resourceFactoryCreator = value ?? throw new ArgumentNullException("ResourceFactoryCreator can't be null!");
        }

        private readonly SemaphoreSlim semaphoreSlim;
        private ChapterWithPage[] chapterWithPages;

        public ComicVisiting(IComicHost host)
        {
            Host = host;
            resourceFactoryCreator = DefaultResourceFactory.Default;
        }

        public async Task LoadAsync(string address)
        {
            this.address = address;
            sourceProvider = Host.GetComicProvider(address);
            entity = await Host.GetComicAsync(address);
            chapterWithPages = new ChapterWithPage[entity.Chapters.Length];
            var ctx = new ResourceFactoryCreateContext
            {
                Address = address,
                SourceProvider = sourceProvider,
                Visiting = this
            };
            resourceFactory = await ResourceFactoryCreator.CreateAsync(ctx);
        }

        public async Task LoadChapterAsync(int index)
        {
            var entity = Entity;
            await semaphoreSlim.WaitAsync();
            try
            {
                if (Entity != entity)//并发控制
                {
                    return;
                }
                var visitor = VisitingInterceptor;
                var chapter = entity.Chapters[index];
                if (visitor != null)
                {
                    var context = new ChapteringVisitingInterceptorContext
                    {
                        Chapter = chapter,
                        Visiting = this
                    };
                    await visitor.LoadingChapterAsync(context);
                }
                var cwp = await sourceProvider.GetPagesAsync(chapter.TargetUrl);
                chapterWithPages[index] = new ChapterWithPage(chapter, cwp);
                if (visitor != null)
                {
                    var ctx = new ChapterVisitingInterceptorContext { Chapter = chapterWithPages[index], Visiting = this };
                    await visitor.LoadedChapterAsync(ctx);
                }
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public async Task<IComicChapterManager> GetChapterManagerAsync(int index)
        {
            await LoadChapterAsync(index);
            var mgr = new ComicChapterManager(chapterWithPages[index], this);
            var inter = VisitingInterceptor;
            if (inter != null)
            {
                await inter.GotChapterManagerAsync(new GotChapterManagerInterceptorContext
                {
                    ChapterManager = mgr,
                    Visiting = this
                });
            }

            return mgr;
        }

        public void Dispose()
        {
            resourceFactory?.Dispose();
            semaphoreSlim.Wait();
            semaphoreSlim.Dispose();
        }
        struct PageBox : IComicVisitPage
        {
            public ComicPage Page { get; set; }

            public Stream Stream { get; set; }
        }
        class ComicChapterManager : IComicChapterManager
        {
            public ComicChapterManager(ChapterWithPage chapterWithPage,
                ComicVisiting comicVisiting)
            {
                ComicVisiting = comicVisiting;
                ChapterWithPage = chapterWithPage;
            }

            public ComicVisiting ComicVisiting { get; }

            public ChapterWithPage ChapterWithPage { get; }

            public async Task<IComicVisitPage> GetVisitPageAsync(int index)
            {
                var page = ChapterWithPage.Pages[index];
                var inter = ComicVisiting.VisitingInterceptor;
                GettingPageInterceptorContext context = null;
                if (inter != null)
                {
                    context = new GettingPageInterceptorContext
                    {
                        Index = index,
                        ChapterManager = this,
                        Page = page,
                        Visiting = ComicVisiting
                    };
                    await inter.GettingPageAsync(context);
                }
                var s = await ComicVisiting.ResourceFactory.GetAsync(page.TargetUrl);
                if (inter != null)
                {
                    await inter.GotPageAsync(context);
                }
                return new PageBox { Page = page, Stream = s };
            }
        }

    }
}
