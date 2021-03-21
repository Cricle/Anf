using Kw.Comic.Engine;
using Kw.Comic.Engine.Easy.Store;
using Kw.Comic.Engine.Easy.Visiting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kw.Comic.Web.Services
{
    public class VisitingManager:IDisposable
    {
        private readonly IServiceProvider serviceProvider;
        private readonly LruCacher<string, IComicVisiting<string>> visitings;
        private readonly SemaphoreSlim semaphoreSlim;

        public VisitingManager(IServiceProvider serviceProvider, int max = 30)
        {
            this.serviceProvider = serviceProvider;
            visitings = new LruCacher<string, IComicVisiting<string>>(max);
            semaphoreSlim = new SemaphoreSlim(1, 1);

            visitings.Removed += Visitings_Removed;
        }

        private void Visitings_Removed(string arg1, IComicVisiting<string> arg2)
        {
            arg2.Dispose();
        }

        private IComicVisiting<string> MakeVisiting()
        {
            var factory = serviceProvider.GetRequiredService<IResourceFactoryCreator<string>>();
            var cacher = serviceProvider.GetRequiredService<ComicDetailCacher>();
            return new StoreVisiting(serviceProvider, factory, cacher);
        }

        public async Task<IComicVisiting<string>> GetVisitingAsync(string address)
        {
            if (visitings.TryGetValue(address, out var val))
            {
                return val;
            }
            await semaphoreSlim.WaitAsync();
            try
            {
                if (visitings.TryGetValue(address, out val))
                {
                    return val;
                }
                var visiting = MakeVisiting();
                await visiting.LoadAsync(address);
                visitings.Add(address, visiting);
                return visiting;
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public void Dispose()
        {
            semaphoreSlim.Wait();
            semaphoreSlim.Dispose();
        }
    }
    public class StoreVisiting : ComicVisiting<string>
    {
        private readonly ComicDetailCacher cacher;

        public StoreVisiting(IServiceProvider host,
            IResourceFactoryCreator<string> resourceFactoryCreator,
            ComicDetailCacher cacher)
            : base(host, resourceFactoryCreator)
        {
            this.cacher = cacher;
        }
        protected override Task<ComicPage[]> GetPagesAsync(ComicChapter chapter)
        {
            var detail = cacher.GetDetail(Address);
            if (detail != null)
            {
                var val = detail.Chapters.FirstOrDefault(x => x.Chapter.TargetUrl == chapter.TargetUrl);
                if (val != null)
                {
                    return Task.FromResult(val.Pages);
                }
            }
            return base.GetPagesAsync(chapter);
        }
        protected override async Task<ComicEntity> MakeEntityAsync(string address)
        {
            var entity = cacher.GetEntity(address);
            if (entity != null)
            {
                return (entity);
            }
            var detail = cacher.GetDetail(address);
            if (detail != null)
            {
                return (detail.Entity);
            }
            var val = await base.MakeEntityAsync(address);
            cacher.SetEntity(address, val);
            return val;
        }
    }
}
