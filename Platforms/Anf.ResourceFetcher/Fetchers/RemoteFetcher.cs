using Anf.ChannelModel.Mongo;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Anf.ResourceFetcher.Fetchers
{
    public class RemoteFetcher : IResourceFetcher
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ComicEngine eng;

        public RemoteFetcher(IServiceScopeFactory serviceScopeFactory, ComicEngine eng)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.eng = eng;
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
                var ctx = context.Copy(context.EntityUrl);
                var ent = await context.Root.FetchEntityAsync(ctx);
                var chp = ent?.Chapters.FirstOrDefault(x => x.TargetUrl == context.Url);
                if (chp is null)
                {
                    return null;
                }
                return new WithPageChapter
                {
                    Pages = entity,
                    CreateTime = now,
                    UpdateTime = now,
                    TargetUrl = context.Url,
                    Title = chp.Title
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
