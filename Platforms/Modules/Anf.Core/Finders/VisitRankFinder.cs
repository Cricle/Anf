﻿using Anf.Core.Models;
using Anf.WebService;
using Ao.Cache.Redis.Finders;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Core.Finders
{
    public class VisitRankFinder : HashCacheFinder<int, RangeVisitEntity>
    {
        private static readonly RedisValue hitColumn = nameof(RangeVisitEntity.HitCount);

        public VisitRankFinder(IDatabase database, ComicRankService rankService, IMemoryCache memoryCache, IOptions<VisitRankFetcherOptions> options, ILogger<VisitRankFinder> logger)
        {
            Database = database;
            RankService = rankService;
            MemoryCache = memoryCache;
            Options = options;
            Logger = logger;
            Build();
        }

        public IDatabase Database { get; }

        public ComicRankService RankService { get; }

        public IMemoryCache MemoryCache { get; }

        public IOptions<VisitRankFetcherOptions> Options { get; }

        public ILogger<VisitRankFinder> Logger { get; }

        public override IDatabase GetDatabase()
        {
            return Database;
        }
        public override async Task<RangeVisitEntity> FindInCahceAsync(int identity)
        {
            var res = await FindAndFlushAsync(identity);
            if (res != null)
            {
                var key = GetEntryKey(identity);
                await Database.HashIncrementAsync(key, hitColumn, 1);
                res.HitCount++;
                MemoryCache.Set(key, res, Options.Value.IntervalTime * 0.2);
            }
            return res;
        }
        public Task<RangeVisitEntity> FindInCahceNoIncHitAsync(int identity)
        {
            return FindAndFlushAsync(identity);
        }
        private async Task<RangeVisitEntity> FindAndFlushAsync(int identity)
        {
            try
            {
                var res = await base.FindInCahceAsync(identity);
                if (res != null)
                {
                    MemoryCache.Set(GetEntryKey(identity), res, Options.Value.IntervalTime);
                }
                return res;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return null;
            }
        }
        protected override async Task<RangeVisitEntity> OnFindInDbAsync(int identity)
        {
            var tructks = await RankService.RangeVisitEntityAsync(0, identity);
            var size = await RankService.SizeVisitAsync();
            return new RangeVisitEntity
            {
                CreateTime = DateTime.Now,
                Size = size,
                EntityTrucks = tructks,
                HitCount = 0
            };
        }
        protected override bool CanRenewal(int identity, HashEntry[] entity)
        {
            return false;
        }
        protected override TimeSpan? GetCacheTime(int identity, RangeVisitEntity entity)
        {
            return null;
        }
    }
}
