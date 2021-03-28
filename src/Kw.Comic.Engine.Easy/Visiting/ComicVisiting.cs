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
    public class ComicVisiting<TResource> : IComicVisiting<TResource>, IDisposable
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

        public async Task<bool> LoadAsync(string address)
        {
            this.address = address;
            sourceProvider = Host.GetComicProvider(address);
            if (sourceProvider==null)
            {
                return false;
            }
            entity = await MakeEntityAsync(address);
            chapterWithPages = new ChapterWithPage[entity.Chapters.Length];
            var ctx = new ResourceFactoryCreateContext<TResource>
            {
                Address = address,
                SourceProvider = sourceProvider,
                Visiting = this
            };
            resourceFactory = await ResourceFactoryCreator.CreateAsync(ctx);
            return true;
        }

        protected virtual Task<ComicEntity> MakeEntityAsync(string address)
        {
            return Host.GetComicAsync(address);
        }
        protected virtual Task<ComicPage[]> GetPagesAsync(ComicChapter chapter)
        {
            return sourceProvider.GetPagesAsync(chapter.TargetUrl);
        }
        public void EraseChapter(int index)
        {
            Interlocked.CompareExchange(ref chapterWithPages[index], null, chapterWithPages[index]);
        }
        public async Task LoadChapterAsync(int index)
        {
            if (chapterWithPages[index] != null)
            {
                return;
            }
            var entity = Entity;
            await semaphoreSlim.WaitAsync();
            try
            {
                if (Entity != entity)//并发控制
                {
                    return;
                }
                if (chapterWithPages[index] != null)
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
                var cwp = await GetPagesAsync(chapter);
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
            var mgr = new ComicChapterManager<TResource>(chapterWithPages[index], this);
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

    }

}
