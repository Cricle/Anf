#define DBSELECT_MSSQL
using Anf.ChannelModel;
using Anf.WebService;
using Ao.Cache.Redis.Finders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Hitokoto.Caching
{
    public class RandomWordResultCacheFinder : HashCacheFinder<int, RandomWordResult>
    {
        private static readonly RedisValue hitColumn = nameof(RandomWordResult.HitCount);

        private static readonly TimeSpan wordCacheTime = TimeSpan.FromHours(1);

        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly IDatabase database;
        private readonly IMemoryCache memoryCache;
        private readonly ILogger<RandomWordResultCacheFinder> logger;

        public IOptions<WordFetchOptions> Options { get; }

        public RandomWordResultCacheFinder(IDatabase database,
            IServiceScopeFactory serviceScopeFactory,
            IMemoryCache memoryCache,
            IOptions<WordFetchOptions> options,
            ILogger<RandomWordResultCacheFinder> logger)
        {
            this.logger = logger;
            this.database = database;
            this.memoryCache = memoryCache;
            Options = options;
            this.serviceScopeFactory = serviceScopeFactory;
            this.database = database;
            Build();
        }
        protected override async Task<RandomWordResult> OnFindInDbAsync(int identity)
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<AnfDbContext>();
            var count = identity <= 0 ? Options.Value.IntervalCount : identity;
            var words = await GetRandomWordsAsync(db, count);
            var res = new RandomWordResult
            {
                CreateTime = DateTime.Now,
                LifeTime = Options.Value.IntervalTime,
                HitCount = 0,
                Words = words
            };
            return res;
        }
        public Task<RandomWordResult> FindInCahceNoIncHitAsync(int identity)
        {
            return FindAndFlushAsync(identity);
        }
        private async Task<RandomWordResult> FindAndFlushAsync(int identity)
        {
            try
            {
                var res = await base.FindInCahceAsync(identity);
                if (res != null)
                {
                    memoryCache.Set(GetEntryKey(identity), res, Options.Value.IntervalTime);
                }
                return res;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return null;
            }
        }
        public Task<RandomWordResult> FindInCahceAsync()
        {
            return FindInCahceAsync(Options.Value.IntervalCount);
        }
        public override async Task<RandomWordResult> FindInCahceAsync(int identity)
        {
            var res = await FindAndFlushAsync(identity);
            if (res != null)
            {
                var key = GetEntryKey(identity);
                await database.HashIncrementAsync(key, hitColumn, 1);
                res.HitCount++;
                memoryCache.Set(key, res, Options.Value.IntervalTime * 0.2);
            }
            return res;
        }

        public Task<List<WordResponse>> GetRandomWordsAsync(AnfDbContext dbContext, int count)
        {
            return dbContext.Words
#if DBSELECT_MYSQL||DBSELECT_MSSQL
                .FromSqlRaw("SELECT t1.* FROM Words t1 JOIN (SELECT RAND() * (SELECT MAX(Id) FROM Words) AS nid) t2 ON t1.Id > t2.nid")
#elif DBSELECT_SQLITE
                .FromSqlRaw("SELECT t1.* FROM Words t1 JOIN (SELECT abs(random() / 9.2233720368547799E+18) * (SELECT MAX(Id) FROM Words) AS nid) t2 ON t1.id > t2.nid limit {0}", count)
#else
#error No random get body
#endif
                .Take(count)
                .Select(x => new WordResponse
                {
                    AuthorId = x.AuthorId,
                    Id = x.Id,
                    CommitType = x.CommitType,
                    CreateTime = x.CreateTime,
                    CreatorId = x.CreatorId,
                    From = x.From,
                    Length = x.Length,
                    Text = x.Text,
                    Type = x.Type,
                    LikeCount = x.LikeCount,
                    VisitCount = x.VisitCount,
                    AuthorName = x.Author == null ? null : x.Author.UserName,
                    CreatorName = x.Creator.UserName
                })
                .ToListAsync();
        }
        protected override TimeSpan? GetCacheTime(int identity, RandomWordResult entity)
        {
            return wordCacheTime;
        }

        public override IDatabase GetDatabase()
        {
            return database;
        }
    }
}
