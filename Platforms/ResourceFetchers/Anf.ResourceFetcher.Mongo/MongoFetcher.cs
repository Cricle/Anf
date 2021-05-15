#pragma warning disable CS0251
using Anf.ChannelModel.Mongo;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anf.ResourceFetcher.Fetchers
{
    public class MongoFetcher : ISingleResourceFetcher
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
            if (context.ProviderFetcher != this&&!context.FetchContext.IsFromCache)
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
            if (context.ProviderFetcher != this && !context.FetchContext.IsFromCache)
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
        private UpdateDefinition<AnfComicEntity> MakeUpdater(AnfComicEntityInfoOnly entity)
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
                .FirstOrDefaultAsync();
            var now = DateTime.Now.Ticks;
            var isUpdate = entity != null;
            var needUpdate = entity != null && (now - entity.UpdateTime) >= fetchOptions.Value.DataTimeout.Ticks;
            AnfComicEntityTruck truck = null;
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
                    truck = await remoteFetcher.FetchEntityAsync(context);
                    if (truck != null)
                    {
                        if (needUpdate)
                        {
                            var updater = MakeUpdater(entity);
                            var chps = truck.Chapters.Select(x => new WithPageChapter
                            {
                                TargetUrl = x.TargetUrl,
                                CreateTime = now,
                                Title = x.Title
                            }).ToArray();
                            var originMap = entity.WithPageChapters.GroupBy(x=>x.TargetUrl)
                                .ToDictionary(x => x.Key,x=>x.First());
                            foreach (var item in chps)
                            {
                                if (originMap.TryGetValue(item.TargetUrl,out var origin))
                                {
                                    item.Pages = origin.Pages;
                                    item.UpdateTime = origin.UpdateTime;
                                    item.RefCount = origin.RefCount;
                                    item.CreateTime = origin.CreateTime;
                                }
                            }
                            updater = updater.Set(x => x.WithPageChapters, chps);
                            var r = await coll.UpdateOneAsync(filter, updater);
                        }
                        else
                        {
                            var val = new AnfComicEntity
                            {
                                ComicUrl = truck.ComicUrl,
                                CreateTime = truck.CreateTime,
                                Descript = truck.Descript,
                                ImageUrl = truck.ImageUrl,
                                Name = truck.Name,
                                RefCount = truck.RefCount,
                                UpdateTime = truck.UpdateTime,
                                WithPageChapters = truck.Chapters.Select(x => new WithPageChapter
                                {
                                    CreateTime = now,
                                    Title = x.Title,
                                    TargetUrl = x.TargetUrl
                                }).ToArray()
                            };
                            await coll.InsertOneAsync(val);
                        }
                    }
                }
            }
            if (truck is null&&entity!=null)
            {
                truck = new AnfComicEntityTruck
                {
                    CreateTime = entity.CreateTime,
                    ComicUrl = entity.ComicUrl,
                    Descript = entity.Descript,
                    ImageUrl = entity.ImageUrl,
                    Name = entity.Name,
                    RefCount = entity.RefCount,
                    UpdateTime = entity.UpdateTime,
                    Chapters = entity.WithPageChapters.Select(x => new ComicChapter
                    {
                        TargetUrl = x.TargetUrl,
                        Title = x.Title
                    }).ToArray()
                };
            }
            return truck;
        }
    }
}

#pragma warning restore CS0251
