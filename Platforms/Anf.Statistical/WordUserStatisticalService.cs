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
    public class WordUserStatisticalService : StatisticalService<AnfWordUserCount, AnfWordUserStatistic>
    {
        private static readonly RedisKey CountKey = "Anf.Statistical.Count.WordUser";

        public WordUserStatisticalService(AnfDbContext dbContext, IDatabase database) : base(dbContext, database)
        {
        }

        protected override RedisKey GetCountsKey()
        {
            return CountKey;
        }

        protected override Task<List<AnfWordUserStatistic>> GetStatisticalsAsync(StatisticLevels type, DateTime left, DateTime right)
        {
            var now = DateTime.Now;
            return DbContext.WordUserCount.AsNoTracking()
                .Where(x => x.Time > left && x.Time <= right)
                .GroupBy(x => x.UserId)
                .Select(x => new AnfWordUserStatistic
                {
                    UserId = x.Key,
                    Time = now,
                    Type = type,
                    Count = x.LongCount()
                }).ToListAsync();
        }
    }
}
