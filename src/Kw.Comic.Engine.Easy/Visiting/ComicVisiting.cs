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
    internal class ComicVisiting<TResource> : IComicVisiting<TResource>, IDisposable
    {
        private string address;
        private ComicEntity entity;
        private IResourceFactoryCreator<TResource> resourceFactoryCreator;
        private IComicSourceProvider sourceProvider;
        private IResourceFactory<TResource> resourceFactory;

        public IServiceProvider Host { get; }
        public IComicSourceProvider SourceProvider => sourceProvider;

        public string Address => address;
        public ComicEntity Entity => entity;

        public IResourceFactory<TResource> ResourceFactory => resourceFactory;

        public IComicVisitingInterceptor<TResource> VisitingInterceptor { get; set; }

        public IResourceFactoryCreator<TResource> ResourceFactoryCreator
        {
            get => resourceFactoryCreator;
            set => resourceFactoryCreator = value ?? throw new ArgumentNullException("ResourceFactoryCreator can't be null!");
        }

        private readonly SemaphoreSlim semaphoreSlim;
        private ChapterWithPage[] chapterWithPages;

        public ComicVisiting(IServiceProvider host, IResourceFactoryCreator<TResource> resourceFactoryCreator)
        {
            if (resourceFactoryCreator is null)
            {
                throw new ArgumentNullException(nameof(resourceFactoryCreator));
            }

            Host = host;
            semaphoreSlim = new SemaphoreSlim(1);
            this.resourceFactoryCreator = resourceFactoryCreator;
        }

        public async Task LoadAsync(string address)
        {
            this.address = address;
            sourceProvider = Host.GetComicProvider(address);
            entity = await Host.GetComicAsync(address);
            chapterWithPages = new ChapterWithPage[entity.Chapters.Length];
            var ctx = new ResourceFactoryCreateContext<TResource>
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
                    var context = new ChapteringVisitingInterceptorContext<TResource>
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
                    var ctx = new ChapterVisitingInterceptorContext<TResource> { Chapter = chapterWithPages[index], Visiting = this };
                    await visitor.LoadedChapterAsync(ctx);
                }
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public async Task<IComicChapterManager<TResource>> GetChapterManagerAsync(int index)
        {
            await LoadChapterAsync(index);
            var mgr = new ComicChapterManager(chapterWithPages[index], this);
            var inter = VisitingInterceptor;
            if (inter != null)
            {
                await inter.GotChapterManagerAsync(new GotChapterManagerInterceptorContext<TResource>
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
        struct PageBox : IComicVisitPage<TResource>
        {
            public ComicPage Page { get; set; }

            public TResource Resource { get; set; }
        }
        class ComicChapterManager : IComicChapterManager<TResource>
        {
            public ComicChapterManager(ChapterWithPage chapterWithPage,
                ComicVisiting<TResource> comicVisiting)
            {
                ComicVisiting = comicVisiting;
                ChapterWithPage = chapterWithPage;
            }

            public ComicVisiting<TResource> ComicVisiting { get; }

            public ChapterWithPage ChapterWithPage { get; }

            public async Task<IComicVisitPage<TResource>> GetVisitPageAsync(int index)
            {
                var page = ChapterWithPage.Pages[index];
                var inter = ComicVisiting.VisitingInterceptor;
                GettingPageInterceptorContext<TResource> context = null;
                if (inter != null)
                {
                    context = new GettingPageInterceptorContext<TResource>
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
                return new PageBox { Page = page, Resource = s };
            }
        }

    }
}
