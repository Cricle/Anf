using Anf.ChannelModel.KeyGenerator;
using Anf.ChannelModel.Mongo;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Anf.ResourceFetcher.Redis
{
    public class InRedisComicService
    {
        private const string ChapterKey = "Anf.ResourceFetcher.Services.AnalysisService.Chapters";
        private const string EntityKey = "Anf.ResourceFetcher.Services.AnalysisService.Entity";
        private const string ChapterMapKey = "Anf.ResourceFetcher.Services.AnalysisService.ChapterMap";

        private readonly IDatabase redisDatabase;
        private readonly IOptions<FetchOptions> fetchOptions;

        public InRedisComicService(IDatabase redisDatabase, IOptions<FetchOptions> fetchOptions)
        {
            this.redisDatabase = redisDatabase;
            this.fetchOptions = fetchOptions;
        }

        private AnfComicEntityTruck AsEntity(HashEntry[] entries)
        {
            if (entries.Length == 0)
            {
                return null;
            }
            var map = entries.ToDictionary();
            T Find<T>(string name)
            {
                if (map.TryGetValue(name,out var val))
                {
                    return val.Get<T>();
                }
                return default;
            }
            return new AnfComicEntityTruck
            {
                ComicUrl = Find<string>(nameof(AnfComicEntityTruck.ComicUrl)),
                Descript = Find<string>(nameof(AnfComicEntityTruck.Descript)),
                ImageUrl = Find<string>(nameof(AnfComicEntityTruck.ImageUrl)),
                Name = Find<string>(nameof(AnfComicEntityTruck.Name)),
                Chapters = Find<ComicChapter[]>(nameof(AnfComicEntityTruck.Chapters)),
                CreateTime = Find<long>(nameof(AnfComicEntityTruck.CreateTime)),
                RefCount = Find<long>(nameof(AnfComicEntityTruck.RefCount)),
                UpdateTime = Find<long>(nameof(AnfComicEntityTruck.UpdateTime)),
            };
        }
        private WithPageChapter AsChapter(HashEntry[] entries)
        {
            if (entries.Length == 0)
            {
                return null;
            }
            var map = entries.ToDictionary();
            T Find<T>(string name)
            {
                if (map.TryGetValue(name, out var val))
                {
                    return val.Get<T>();
                }
                return default;
            }
            return new WithPageChapter
            {
                Pages = Find<ComicPage[]>(nameof(WithPageChapter.Pages)),
                TargetUrl = Find<string>(nameof(WithPageChapter.TargetUrl)),
                Title = Find<string>(nameof(WithPageChapter.Title)),
                CreateTime = Find<long>(nameof(AnfComicEntityTruck.CreateTime)),
                RefCount = Find<long>(nameof(AnfComicEntityTruck.RefCount)),
                UpdateTime = Find<long>(nameof(AnfComicEntityTruck.UpdateTime))
            };
        }
        private HashEntry[] AsHash(AnfComicEntityTruck value)
        {
            var chapterBytes = JsonSerializer.SerializeToUtf8Bytes(value.Chapters);
            return new HashEntry[]
            {
                new HashEntry(nameof(AnfComicEntityTruck.ComicUrl),value.ComicUrl),
                new HashEntry(nameof(AnfComicEntityTruck.Name),value.Name),
                new HashEntry(nameof(AnfComicEntityTruck.Descript),value.Descript),
                new HashEntry(nameof(AnfComicEntityTruck.ImageUrl),value.ImageUrl),
                new HashEntry(nameof(AnfComicEntityTruck.Chapters),chapterBytes),

                new HashEntry(nameof(AnfComicEntityTruck.RefCount),value.RefCount),
                new HashEntry(nameof(AnfComicEntityTruck.CreateTime),value.CreateTime),
                new HashEntry(nameof(AnfComicEntityTruck.UpdateTime),value.UpdateTime),
            };
        }
        private HashEntry[] AsHash(WithPageChapter value)
        {
            var pageBytes = JsonSerializer.SerializeToUtf8Bytes(value.Pages);
            return new HashEntry[]
            {
                new HashEntry(nameof(WithPageChapter.TargetUrl),value.TargetUrl),
                new HashEntry(nameof(WithPageChapter.Title),value.Title),
                new HashEntry(nameof(WithPageChapter.Pages),pageBytes),
                new HashEntry(nameof(WithPageChapter.RefCount),value.RefCount),
                new HashEntry(nameof(WithPageChapter.CreateTime),value.CreateTime),
                new HashEntry(nameof(WithPageChapter.UpdateTime),value.UpdateTime),
            };
        }
        public async Task<string> GetChapterNameAsync(string url)
        {
            var mkey = RedisKeyGenerator.Concat(ChapterMapKey, url);
            var res = await redisDatabase.StringGetAsync(mkey);
            return res.Get<string>();
        }
        public Task UpdateEntityAsync(AnfComicEntityTruck value)
        {
            var key = RedisKeyGenerator.Concat(EntityKey, value.ComicUrl);
            var hashs = AsHash(value);

            var batch = redisDatabase.CreateBatch();
            var tasks = new List<Task>(2 + value.Chapters.Length);
            tasks.Add(batch.HashSetAsync(key, hashs));
            tasks.Add(batch.KeyExpireAsync(key, fetchOptions.Value.CacheTimeout));
            foreach (var item in value.Chapters)
            {
                var mkey = RedisKeyGenerator.Concat(ChapterMapKey, item.TargetUrl);
                tasks.Add(batch.StringSetAsync(mkey,item.Title,fetchOptions.Value.ChapterMapTimeout));
            }
            batch.Execute();
            return Task.WhenAll(tasks.ToArray());
        }
        public async Task UpdateChapterAsync(WithPageChapter value)
        {
            var key = RedisKeyGenerator.Concat(ChapterKey, value.TargetUrl);
            var hashs = AsHash(value);
            await redisDatabase.HashSetAsync(key, hashs);
            await redisDatabase.KeyExpireAsync(key, fetchOptions.Value.CacheTimeout);
        }
        public async Task<AnfComicEntityTruck> GetEntityAsync(string url)
        {
            var key = RedisKeyGenerator.Concat(EntityKey, url);
            var val = await redisDatabase.HashGetAllAsync(key);
            return AsEntity(val);
        }
        public async Task<WithPageChapter> GetChapterAsync(string url)
        {
            var key = RedisKeyGenerator.Concat(ChapterKey, url);
            var val = await redisDatabase.HashGetAllAsync(key);
            return AsChapter(val);
        }
    }
}
