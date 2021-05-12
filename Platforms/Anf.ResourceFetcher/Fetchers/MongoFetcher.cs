#pragma warning disable CS0251
using Anf.ChannelModel.Mongo;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Anf.ResourceFetcher.Fetchers
{
    public class MongoFetcher : IResourceFetcher
    {
        private readonly IMongoClient mongoClient;
        private readonly RemoteFetcher remoteFetcher;
        private readonly IOptions<FetchOptions> fetchOptions;

        public MongoFetcher(IMongoClient mongoClient, RemoteFetcher remoteFetcher, IOptions<FetchOptions> fetchOptions)
        {
            this.mongoClient = mongoClient;
            this.remoteFetcher = remoteFetcher;
            this.fetchOptions = fetchOptions;
        }

        public async Task DoneFetchChapterAsync(IValueResourceMonitorContext<WithPageChapter> context)
        {
            if (context.ProviderFetcher != this)
            {
                var uri = context.Value.TargetUrl;
                var coll = mongoClient.GetComicEntityCollection();
                var filter = Builders<AnfComicEntity>.Filter.ElemMatch(x => x.WithPageChapters, x => x.TargetUrl == uri);
                var updater = MakeUpdater(context.Value);
                await coll.UpdateOneAsync(filter,updater);
            }
        }

        public async Task DoneFetchEntityAsync(IValueResourceMonitorContext<AnfComicEntityTruck> context)
        {
            if (context.ProviderFetcher != this)
            {
                var coll = mongoClient.GetComicEntityCollection();
                var filter = Builders<AnfComicEntity>.Filter.Eq(x => x.ComicUrl, context.Value.ComicUrl);
                var updater = MakeUpdater(context.Value);
                await coll.UpdateOneAsync(filter, updater);
            }
        }
        private UpdateDefinition<AnfComicEntity> MakeUpdater(WithPageChapter entity)
        {
            var now = DateTime.Now.Ticks;
            return Builders<AnfComicEntity>.Update
                .Set(x => x.WithPageChapters[-1].Pages, entity.Pages)
                .Set(x => x.WithPageChapters[-1].TargetUrl, entity.TargetUrl)
                .Set(x => x.WithPageChapters[-1].Title, entity.Title)
                .Set(x => x.WithPageChapters[-1].UpdateTime, now);
        }
        public async Task<WithPageChapter> FetchChapterAsync(IResourceFetchContext context)
        {
            var coll = mongoClient.GetComicEntityCollection();
            var filter = Builders<AnfComicEntity>.Filter.ElemMatch(x => x.WithPageChapters, x => x.TargetUrl == context.Url);
            var entity = await coll.Find(filter)
                .Project(x => x.WithPageChapters.FirstOrDefault(y => y.TargetUrl == context.Url))
                .FirstOrDefaultAsync();
            var now = DateTime.Now.Ticks;
            var isUpdate = entity != null;
            var needUpdate = entity != null && (now - entity.UpdateTime) >= fetchOptions.Value.DataTimeout.Ticks;
            if (entity is null)
            {
                return null;
            }
            if (needUpdate)
            {
                using (var locker = await context.CreateChapterLockerAsync())
                {
                    if (!locker.IsAcquired)
                    {
                        if (context.RequireReloopFetcher != this)
                        {
                            context.SetRequireReloop();
                        }
                        return null;
                    }
                    entity = await remoteFetcher.FetchChapterAsync(context);
                    if (entity != null)
                    {
                        var updater = MakeUpdater(entity);
                        await coll.UpdateOneAsync(filter, updater);
                    }
                }
            }
            return entity;
        }
        private UpdateDefinition<AnfComicEntity> MakeUpdater(AnfComicEntityTruck entity)
        {
            var now = DateTime.Now.Ticks;
            return Builders<AnfComicEntity>.Update
                .Set(x => x.ComicUrl, entity.ComicUrl)
                .Set(x => x.Descript, entity.Descript)
                .Set(x => x.Name, entity.Name)
                .Set(x => x.UpdateTime, now);
        }
        public async Task<AnfComicEntityTruck> FetchEntityAsync(IResourceFetchContext context)
        {
            var coll = mongoClient.GetComicEntityCollection();
            var filter = Builders<AnfComicEntity>.Filter.Eq(x => x.ComicUrl, context.Url);
            var entity = await coll.Find(filter)
                .Project(x => new AnfComicEntityTruck
                {
                    ComicUrl = x.ComicUrl,
                    CreateTime = x.CreateTime,
                    Descript = x.Descript,
                    ImageUrl = x.ImageUrl,
                    Name = x.Name,
                    RefCount = x.RefCount,
                    UpdateTime = x.UpdateTime,
                    Chapters = x.WithPageChapters.Select(y => new ComicChapter
                    {
                        TargetUrl = y.TargetUrl,
                        Title = y.Title
                    }).ToArray()
                }).FirstOrDefaultAsync();
            var now = DateTime.Now.Ticks;
            var isUpdate = entity != null;
            var needUpdate = entity != null && (now - entity.UpdateTime) >= fetchOptions.Value.DataTimeout.Ticks;

            if (entity is null || needUpdate)
            {
                using (var locker =await context.CreateEntityLockerAsync())
                {
                    if (!locker.IsAcquired)
                    {
                        if (context.RequireReloopFetcher != this)
                        {
                            context.SetRequireReloop();
                        }
                        return null;
                    }
                    entity = await remoteFetcher.FetchEntityAsync(context);
                    if (entity != null)
                    {
                        if (needUpdate)
                        {
                            var updater = MakeUpdater(entity);
                            var res = await coll.UpdateOneAsync(filter, updater);
                        }
                        else
                        {
                            var val = new AnfComicEntity
                            {
                                ComicUrl = entity.ComicUrl,
                                CreateTime = entity.CreateTime,
                                Descript = entity.Descript,
                                ImageUrl = entity.ImageUrl,
                                Name = entity.Name,
                                RefCount = entity.RefCount,
                                UpdateTime = entity.UpdateTime
                            };
                            await coll.InsertOneAsync(val);
                        }
                    }
                }
            }
            return entity;
        }
    }
}

#pragma warning restore CS0251
