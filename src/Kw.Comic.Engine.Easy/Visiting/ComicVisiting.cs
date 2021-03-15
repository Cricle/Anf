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
        private IComicSourceProvider sourceProvider;

        private SharedComic sharedComic;
        private readonly RecyclableMemoryStreamManager recyclableMemoryStreamManager;
        private readonly IComicHost host;

        public string Address => address;
        public ComicEntity Entity => entity;

        public int? SharedCapacity { get; set; }

        public ISharedComic SharedComic => sharedComic;

        private readonly SemaphoreSlim semaphoreSlim;
        private readonly ChapterWithPage[] chapterWithPages;

        public ComicVisiting(RecyclableMemoryStreamManager recyclableMemoryStreamManager, IComicHost host)
        {
            this.recyclableMemoryStreamManager = recyclableMemoryStreamManager;
            this.host = host;
        }

        public async Task LoadAsync(string address)
        {
            this.address = address;
            sourceProvider = host.GetComicProvider(address);
            entity = await host.GetComicAsync(address);
            sharedComic?.Dispose();
            sharedComic = new SharedComic(sourceProvider, recyclableMemoryStreamManager, SharedCapacity);
        }

        public async Task LoadChapterAsync(int index)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                var chapter = entity.Chapters[index];
                var cwp = await sourceProvider.GetPagesAsync(chapter.TargetUrl);
                chapterWithPages[index] = new ChapterWithPage(chapter, cwp);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public async Task<IComicChapterManager> GetChapterManagerAsync(int index)
        {
            await LoadChapterAsync(index);
            return new ComicChapterManager(SharedComic, chapterWithPages[index]);
        }

        public void Dispose()
        {
            sharedComic?.Dispose();
            semaphoreSlim.Wait();
            semaphoreSlim.Dispose();
        }
    }
}
