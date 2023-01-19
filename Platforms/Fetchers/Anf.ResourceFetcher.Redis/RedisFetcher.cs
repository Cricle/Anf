using Anf.ChannelModel.Mongo;
using Anf.ResourceFetcher.Fetchers;
using System.Linq;
using System.Threading.Tasks;

namespace Anf.ResourceFetcher.Redis
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

        public Task DoneFetchChapterAsync(IValueResourceMonitorContext<WithPageChapter>[] context)
        {
            var enableCtx = context.Where(x => x.ProviderFetcher != this).ToArray();
            if (enableCtx.Length != 0)
            {
                return inRedisComicService.UpdateChapterAsync(enableCtx.Select(x => x.Value));
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

        public Task DoneFetchEntityAsync(IValueResourceMonitorContext<AnfComicEntityTruck>[] context)
        {
            var enableCtx = context.Where(x => x.ProviderFetcher != this).ToArray();
            if (enableCtx.Length != 0)
            {
                return inRedisComicService.UpdateEntityAsync(enableCtx.Select(x => x.Value));
            }
            return Task.CompletedTask;
        }

        public async Task<WithPageChapter> FetchChapterAsync(IResourceFetchContext context)
        {
            var chp = await inRedisComicService.GetChapterAsync(context.Url);
            if (chp != null)
            {
                context.SetIsCache();
            }
            return chp;
        }

        public async Task<WithPageChapter[]> FetchChapterAsync(IResourceFetchContext[] context)
        {
            var urls = context.Select(x => x.Url).ToArray();
            var datas = await inRedisComicService.BatchGetChapterAsync(urls);
            foreach (var item in context)
            {
                item.SetIsCache();
            }
            return datas;
        }

        public async Task<AnfComicEntityTruck> FetchEntityAsync(IResourceFetchContext context)
        {
            var entity = await inRedisComicService.GetEntityAsync(context.Url);
            if (entity != null)
            {
                context.SetIsCache();
            }
            return entity;
        }

        public async Task<AnfComicEntityTruck[]> FetchEntityAsync(IResourceFetchContext[] context)
        {
            var urls = context.Select(x => x.Url).ToArray();
            var datas = await inRedisComicService.BatchGetEntityAsync(urls);
            foreach (var item in context)
            {
                item.SetIsCache();
            }
            return datas;
        }
    }
}
