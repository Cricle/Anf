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
    public class VisitStatisticalService : StatisticalService<AnfVisitCount, AnfVisitStatistic>
    {
        private static readonly RedisKey CountKey = "Anf.Statistical.Count.Visit";

        public VisitStatisticalService(AnfDbContext dbContext, IDatabase database) : base(dbContext, database)
        {
        }

        protected override RedisKey GetCountsKey()
        {
            return CountKey;
        }

        protected override Task<List<AnfVisitStatistic>> GetStatisticalsAsync(StatisticLevels type, DateTime left, DateTime right)
        {
            var now = DateTime.Now;
            return DbContext.Visits.AsNoTracking()
                .Where(x => x.Time > left && x.Time <= right)
                .GroupBy(x => x.Address)
                .Select(x => new AnfVisitStatistic
                {
                    Address = x.Key,
                    Time = now,
                    Type = type,
                    Count = x.LongCount()
                }).ToListAsync();
        }
    }
}
