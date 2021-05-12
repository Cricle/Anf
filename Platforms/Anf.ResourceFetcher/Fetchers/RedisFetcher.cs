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
        public Task<WithPageChapter> FetchChapterAsync(IResourceFetchContext context)
        {
            return inRedisComicService.GetChapterAsync(context.Url);
        }

        public Task<AnfComicEntityTruck> FetchEntityAsync(IResourceFetchContext context)
        {
            return inRedisComicService.GetEntityAsync(context.Url);
        }
    }
}
