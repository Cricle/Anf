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
    public class WorkReadStatisticalService : StatisticalService<AnfWordReadCount, AnfWorkReadStatistic>
    {
        private static readonly RedisKey CountKey = "Anf.Statistical.Count.WorkRead";

        public WorkReadStatisticalService(AnfDbContext dbContext, IDatabase database) : base(dbContext, database)
        {
        }

        protected override RedisKey GetCountsKey()
        {
            return CountKey;
        }

        protected override Task<List<AnfWorkReadStatistic>> GetStatisticalsAsync(StatisticLevels type, DateTime left, DateTime right)
        {
            var now = DateTime.Now;
            return DbContext.WordReadStatistics.AsNoTracking()
                .Where(x => x.Time > left && x.Time <= right)
                .GroupBy(x => x.WordId)
                .Select(x => new AnfWorkReadStatistic
                {
                    WordId = x.Key,
                    Time = now,
                    Type = type,
                    Count = x.LongCount()
                }).ToListAsync();
        }
    }
}
