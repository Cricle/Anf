﻿using Anf.ChannelModel.Mongo;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Anf.ResourceFetcher.Fetchers
{
    public class RootFetcher : ResourceFetchGroup, IRootFetcher
    {
        readonly struct FetchResult<T>
        {
            public readonly IResourceFetchContext ResourceFetchContext;
            public readonly ISingleResourceFetcher TriggerFetcher;
            public readonly T Value;

            public FetchResult(IResourceFetchContext resourceFetchContext, ISingleResourceFetcher triggerFetcher, T value)
            {
                ResourceFetchContext = resourceFetchContext;
                TriggerFetcher = triggerFetcher;
                Value = value;
            }
        }
        private readonly IResourceLockerFactory resourceLockerFactory;

        public RootFetcher(IResourceLockerFactory resourceLockerFactory)
        {
            this.resourceLockerFactory = resourceLockerFactory;
        }
        public async Task<WithPageChapter[]> FetchChaptersAsync(FetchChapterIdentity[] identities)
        {
            var ctx = identities.Select(x => new ResourceFetchContext(resourceLockerFactory, x.Url, x.ReloopFetcher, this, x.EntityUrl))
                .ToArray();
            var res = await base.FetchChapterAsync(ctx);
            var requiredReloop = ctx.Where(x => x.RequireReloop).ToArray();
            if (requiredReloop.Length != 0)
            {
                var thenCtx = requiredReloop.Select(x => new FetchChapterIdentity(x.Url, x.EntityUrl)).ToArray();
                var thenRes = await FetchChaptersAsync(thenCtx);
                return res.Concat(thenRes).ToArray();
            }
            return res;
        }
        public async Task<AnfComicEntityTruck[]> FetchEntitysAsync(FetchChapterIdentity[] identities)
        {
            var ctx = identities.Select(x => new ResourceFetchContext(resourceLockerFactory, x.Url, x.ReloopFetcher, this, x.EntityUrl))
                .ToArray();
            var res = await base.FetchEntityAsync(ctx);
            var requiredReloop = ctx.Where(x => x.RequireReloop).ToArray();
            if (requiredReloop.Length != 0)
            {
                var thenCtx = requiredReloop.Select(x => new FetchChapterIdentity(x.Url, x.EntityUrl)).ToArray();
                var thenRes = await FetchEntitysAsync(thenCtx);
                return res.Concat(thenRes).ToArray();
            }
            return res;
        }
        public async Task<WithPageChapter> FetchChapterAsync(string url, string entityUrl)
        {
            var val = await FetchAsync(url, (x, y) => x.FetchChapterAsync(y), entityUrl);
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
                return val.Value;
            }
            return null;
        }
        private async Task<FetchResult<T>> FetchAsync<T>(string url,
            Func<ISingleResourceFetcher, IResourceFetchContext, Task<T>> map,
            string entityUrl,
            ISingleResourceFetcher requiredReloopFetcher = null)
        {
            var ctx = new ResourceFetchContext(resourceLockerFactory, url, requiredReloopFetcher, this, entityUrl);
            for (int i = 0; i < Count; i++)
            {
                var fetcher = this[i];
                var val = await map(fetcher, ctx);
                if (val != null)
                {
                    return new FetchResult<T>(ctx, fetcher, val);
                }
                if (ctx.RequireReloop)
                {
                    return await FetchAsync(url, map, entityUrl, fetcher);
                }
            }
            return default;
        }

        public async Task<AnfComicEntityTruck> FetchEntityAsync(string url)
        {
            var val = await FetchAsync(url, (x, y) => x.FetchEntityAsync(y), url);
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
                return val.Value;
            }
            return null;
        }
    }
}
