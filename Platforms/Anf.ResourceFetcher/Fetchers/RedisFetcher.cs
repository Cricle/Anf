using Anf.ChannelModel.KeyGenerator;
using Anf.ChannelModel.Mongo;
using Anf.ResourceFetcher.Services;
using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Anf.ResourceFetcher.Fetchers
{
    public class RedisFetcher : IResourceFetcher
    {
        private readonly InRedisComicService inRedisComicService;

        public RedisFetcher(InRedisComicService inRedisComicService)
        {
            this.inRedisComicService = inRedisComicService;
        }

        public Task DoneFetchChapterAsync(IValueResourceMonitorContext<WithPageChapter> context)
        {
            if (context.ProviderFetcher != this)
            {
                return inRedisComicService.UpdateChapterAsync(context.Value);
            }
            return Task.CompletedTask;
        }

        public Task DoneFetchEntityAsync(IValueResourceMonitorContext<AnfComicEntityTruck> context)
        {
            if (context.ProviderFetcher != this)
            {
                return inRedisComicService.UpdateEntityAsync(context.Value);
            }
            return Task.CompletedTask;
        }
        public async Task<WithPageChapter> FetchChapterAsync(IResourceFetchContext context)
        {
            var chp=await inRedisComicService.GetChapterAsync(context.Url);
            if (chp!=null)
            {
                context.SetIsCache();
            }
            return chp;
        }

        public async Task<AnfComicEntityTruck> FetchEntityAsync(IResourceFetchContext context)
        {
            var entity=await inRedisComicService.GetEntityAsync(context.Url);
            if (entity!=null)
            {
                context.SetIsCache();
            }
            return entity;
        }
    }
}
