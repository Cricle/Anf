using Anf.ChannelModel.Mongo;
using RedLockNet;
using System;
using System.Threading.Tasks;

namespace Anf.ResourceFetcher.Fetchers
{
    public class RootFetcher : ResourceFetchGroup, IRootFetcher
    {
        readonly struct FetchResult<T>
        {
            public readonly IResourceFetchContext ResourceFetchContext;
            public readonly IResourceFetcher TriggerFetcher;
            public readonly T Value;

            public FetchResult(IResourceFetchContext resourceFetchContext,IResourceFetcher triggerFetcher, T value)
            {
                ResourceFetchContext = resourceFetchContext;
                TriggerFetcher = triggerFetcher;
                Value = value;
            }
        }
        private readonly IDistributedLockFactory distributeLockFactory;

        public RootFetcher(IDistributedLockFactory distributeLockFactory)
        {
            this.distributeLockFactory = distributeLockFactory;
        }

        public async Task<WithPageChapter> FetchChapterAsync(string url)
        {
            var val = await FetchAsync(url, (x, y) => x.FetchChapterAsync(y));
            if (val.Value != null)
            {
                var ctx = new ChapterResourceMonitorContext
                {
                    FetchContext = val.ResourceFetchContext,
                    ProviderFetcher = val.TriggerFetcher,
                    Url = url,
                    Value = val.Value
                };
                await DoneFetchChapterAsync(ctx);
            }
            return null;
        }
        private async Task<FetchResult<T>> FetchAsync<T>(string url,
            Func<IResourceFetcher,IResourceFetchContext, Task<T>> map,
            IResourceFetcher requiredReloopFetcher=null)
        {
            var ctx = new ResourceFetchContext(distributeLockFactory, url, requiredReloopFetcher, this);
            for (int i = 0; i < Count; i++)
            {
                var fetcher = this[i];
                var val = await map(fetcher,ctx);
                if (val!=null)
                {
                    return new FetchResult<T>(ctx,fetcher, val);
                }
                if (ctx.RequireReloop)
                {
                    return await FetchAsync(url,map, fetcher);
                }
            }
            return default;
        }

        public async Task<AnfComicEntityTruck> FetchEntityAsync(string url)
        {
            var val = await FetchAsync(url, (x, y) => x.FetchEntityAsync(y));
            if (val.Value != null)
            {
                var ctx = new EntityResourceMonitorContext
                {
                    FetchContext = val.ResourceFetchContext,
                    ProviderFetcher = val.TriggerFetcher,
                    Url = url,
                    Value = val.Value
                };
                await DoneFetchEntityAsync(ctx);
            }
            return null;
        }
    }
}
