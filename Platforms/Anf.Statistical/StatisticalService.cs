using Anf.WebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Anf.ChannelModel.Entity;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;
using ValueBuffer;
using EFCore.BulkExtensions;

namespace Anf.Statistical
{
    public class StatisticalService
    {
        private static readonly RedisKey StatisticalVisitKey = "Anf.Statistical.VisitList";
        private static readonly RedisKey StatisticalSearchey = "Anf.Statistical.SearchList";

        public StatisticalService(AnfDbContext dbContext, IDatabase database, ILogger<StatisticalService> logger)
        {
            DbContext = dbContext;
            Database = database;
            Logger = logger;
        }

        public AnfDbContext DbContext { get; }

        public IDatabase Database { get; }

        public ILogger<StatisticalService> Logger { get; }

        public Task<long> AddVisitAsync(AnfComicVisit visit)
        {
            var str = JsonSerializer.Serialize(visit);

            return Database.ListLeftPushAsync(StatisticalVisitKey, str);
        }
        public Task<long> AddSearchAsync(AnfComicSearch search)
        {
            var str = JsonSerializer.Serialize(search);

            return Database.ListLeftPushAsync(StatisticalSearchey, str);
        }
        private async Task<long> StoreAsync<TCount>(RedisKey popKey,int size)
            where TCount : class
        {
            using (var list = new ValueList<TCount>())
            {
                for (int i = 0; i < size; i++)
                {
                    var ds = await Database.ListLeftPopAsync(popKey);
                    if (!ds.HasValue)
                    {
                        break;
                    }
                    var data = JsonSerializer.Deserialize<TCount>(ds);
                    list.Add(data);
                }
                if (list.Size != 0)
                {
                    await DbContext.Set<TCount>().AddRangeAsync(list.ToArray());
                }
                return list.Size;
            }
        }

        public Task<long> StoreVisitsAsync(int size)
        {
           return StoreAsync<AnfComicVisit>(StatisticalVisitKey, size);
        }
        public Task<long> StoreSearchsAsync(int size)
        {
            return StoreAsync<AnfComicSearch>(StatisticalSearchey, size);
        }

        public async Task<int> SaveVisitAsync(StatisticLevels type, DateTime left, DateTime right)
        {
            DbContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(10));
            var now = DateTime.Now;
            var data = await DbContext.Visits.AsNoTracking()
                .Where(x => x.Time > left && x.Time <= right)
                .GroupBy(x => x.Address)
                .Select(x => new AnfComicVisitRank
                {
                    Address = x.Key,
                    CreateTime = now,
                    Type = type,
                    VisitCount = x.LongCount()
                }).ToListAsync();
            if (data.Count != 0)
            {
                await DbContext.BulkInsertAsync(data);
            }
            Logger.LogInformation("Alread store {0} count visit rank!",data.Count);
            return data.Count;
        }
        public async Task<int> SaveSearchAsync(StatisticLevels type, DateTime left, DateTime right)
        {
            DbContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(10));
            var now = DateTime.Now;
            var data = await DbContext.Searchs.AsNoTracking()
                .Where(x => x.Time > left && x.Time <= right)
                .GroupBy(x => x.Content)
                .Select(x => new AnfComicSearchRank
                {
                    Content = x.Key,
                    CreateTime = now,
                    Type = type,
                    VisitCount = x.LongCount()
                }).ToListAsync();
            if (data.Count != 0)
            {
                await DbContext.BulkInsertAsync(data);
            }
            Logger.LogInformation("Alread store {0} count search rank!", data.Count);
            return data.Count;
        }
    }
}
