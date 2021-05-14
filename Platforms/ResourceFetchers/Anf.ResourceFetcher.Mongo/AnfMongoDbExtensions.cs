using Anf.ChannelModel.Mongo;
using Anf.ResourceFetcher.Caching;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoDB.Driver
{
    public static class AnfMongoDbExtensions
    {
        public static IMongoCollection<AnfComicEntity> GetComicEntityCollection(this IMongoClient client)
        {
            var db = client.GetDatabase(AnfMongoKeys.ReadingDb);
            return db.GetCollection<AnfComicEntity>(AnfMongoKeys.ComicCollection);
        }
        public static async Task InitMongoAsync(IServiceScope scope)
        {
            try
            {
                var mongoClient = scope.ServiceProvider.GetRequiredService<IMongoClient>();
                {
                    var coll = mongoClient.GetComicEntityCollection();
                    var idxs = await coll.Indexes.ListAsync();
                    if (idxs.ToList().Count <= 1)
                    {
                        var idx = Builders<AnfComicEntity>.IndexKeys
                            .Descending(x => x.ComicUrl);
                        await coll.Indexes.CreateOneAsync(new CreateIndexModel<AnfComicEntity>(idx, new CreateIndexOptions { Unique = true }));
                        idx = Builders<AnfComicEntity>.IndexKeys
                            .Descending(x => x.RefCount)
                            .Descending(x => x.CreateTime)
                            .Descending(nameof(AnfComicEntity.WithPageChapters) + "." + nameof(WithPageChapter.TargetUrl))
                            .Descending(nameof(AnfComicEntity.WithPageChapters) + "." + nameof(WithPageChapter.RefCount));
                        await coll.Indexes.CreateOneAsync(new CreateIndexModel<AnfComicEntity>(idx));
                    }
                }

            }
            finally
            {
                scope.Dispose();
            }
        }

    }
}
