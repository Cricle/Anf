using Anf.WebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Anf.ChannelModel.Entity;
using StackExchange.Redis;

namespace Anf.Statistical
{
    public class SearchStatisticalService : StatisticalService<AnfSearchCount, AnfSearchStatistic>
    {
        private static readonly RedisKey CountKey = "Anf.Statistical.Count.Search";

        public SearchStatisticalService(AnfDbContext dbContext, IDatabase database) : base(dbContext, database)
        {
        }

        protected override RedisKey GetCountsKey()
        {
            return CountKey;
        }

        protected override Task<List<AnfSearchStatistic>> GetStatisticalsAsync(StatisticLevels type, DateTime left, DateTime right)
        {
            var now = DateTime.Now;
            return DbContext.Searchs.AsNoTracking()
                .Where(x => x.Time > left && x.Time <= right)
                .GroupBy(x => x.Content)
                .Select(x => new AnfSearchStatistic
                {
                    Content = x.Key,
                    Time = now,
                    Type = type,
                    Count = x.LongCount()
                }).ToListAsync();
        }
    }
}
