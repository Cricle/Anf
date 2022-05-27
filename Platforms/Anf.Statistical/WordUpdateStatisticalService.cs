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
    public class WordUpdateStatisticalService : StatisticalService<AnfWordUpdateCount, AnfWordUpdateStatistic>
    {
        private static readonly RedisKey CountKey = "Anf.Statistical.Count.WordUpdate";

        public WordUpdateStatisticalService(AnfDbContext dbContext, IDatabase database) : base(dbContext, database)
        {
        }

        protected override RedisKey GetCountsKey()
        {
            return CountKey;
        }

        protected override async Task<List<AnfWordUpdateStatistic>> GetStatisticalsAsync(StatisticLevels type, DateTime left, DateTime right)
        {
            var now = DateTime.Now;
            var count = await DbContext.WordUpdateCount.AsNoTracking()
                .Where(x => x.Time > left && x.Time <= right)
                .LongCountAsync();
            return new List<AnfWordUpdateStatistic>(1)
            {
                new AnfWordUpdateStatistic
                {
                     Count=count,
                     Time=now,
                     Type=type,
                }
            };
        }
    }
}
