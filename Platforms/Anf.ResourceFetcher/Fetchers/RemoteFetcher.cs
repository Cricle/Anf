#pragma warning disable CS0251
using Anf.ChannelModel.Mongo;
using Anf.ResourceFetcher.Services;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Anf.ResourceFetcher.Fetchers
{
    public class RemoteFetcher : IResourceFetcher
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ComicEngine eng;
        private readonly InRedisComicService inRedisComicService;

        public RemoteFetcher(IServiceScopeFactory serviceScopeFactory, ComicEngine eng, InRedisComicService inRedisComicService)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.eng = eng;
            this.inRedisComicService = inRedisComicService;
        }

        public Task DoneFetchChapterAsync(IValueResourceMonitorContext<WithPageChapter> context)
        {
            return Task.CompletedTask;
        }

        public Task DoneFetchEntityAsync(IValueResourceMonitorContext<AnfComicEntityTruck> context)
        {
            return Task.CompletedTask;
        }

        public async Task<WithPageChapter> FetchChapterAsync(IResourceFetchContext context)
        {
            var type = eng.GetComicSourceProviderType(context.Url);
            if (type is null)
            {
                return null;
            }
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var ser = (IComicSourceProvider)scope.ServiceProvider.GetRequiredService(type.ProviderType);
                var entity = await ser.GetPagesAsync(context.Url);
                var now = DateTime.Now.Ticks;
                var titleName = await inRedisComicService.GetChapterNameAsync(context.Url);
                if (string.IsNullOrEmpty(titleName))
                {
                    if (string.IsNullOrEmpty(context.EntityUrl))
                    {
                        return null;
                    }
                    var ctx = context.Copy(context.EntityUrl);
                    var ent = await context.Root.FetchEntityAsync(ctx);
                    var chp = ent?.Chapters.FirstOrDefault(x => x.TargetUrl == context.Url);
                    if (chp is null)
                    {
                        return null;
                    }
                    titleName = chp.Title;
                }
                return new WithPageChapter
                {
                    Pages = entity,
                    CreateTime = now,
                    UpdateTime = now,
                    TargetUrl = context.Url,
                    Title = titleName
                };
            }
        }

        public async Task<AnfComicEntityTruck> FetchEntityAsync(IResourceFetchContext context)
        {
            var type = eng.GetComicSourceProviderType(context.Url);
            if (type is null)
            {
                return null;
            }
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var ser = (IComicSourceProvider)scope.ServiceProvider.GetRequiredService(type.ProviderType);
                var entity = await ser.GetChaptersAsync(context.Url);
                var now = DateTime.Now.Ticks;
                return new AnfComicEntityTruck
                {
                    Chapters = entity.Chapters,
                    ComicUrl = entity.ComicUrl,
                    CreateTime = now,
                    Descript = entity.Descript,
                    ImageUrl = entity.ImageUrl,
                    UpdateTime = now,
                    Name = entity.Name,
                };
            }
        }
    }
}

#pragma warning restore CS0251
