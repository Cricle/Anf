using Anf.WebService;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Anf.ChannelModel.Entity;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;
using EFCore.BulkExtensions;

namespace Anf.Statistical
{
    public abstract class StatisticalService<TEntity,TStatistical>
        where TEntity : AnfCount
        where TStatistical : AnfStatistic
    {
        protected StatisticalService(AnfDbContext dbContext, IDatabase database)
        {
            DbContext = dbContext;
            Database = database;
        }

        public AnfDbContext DbContext { get; }

        public IDatabase Database { get; }

        protected abstract RedisKey GetCountsKey();

        public Task<long> AddAsync(TEntity visit)
        {
            var str = Serialize(visit);

            return Database.ListLeftPushAsync(GetCountsKey(), str);
        }
        protected virtual RedisValue Serialize(TEntity entity)
        {
            return JsonSerializer.Serialize(entity);
        }
        protected virtual TEntity Deserialize(in RedisValue value)
        {
            return JsonSerializer.Deserialize<TEntity>(value);
        }
        public async Task<long> StoreToDbAsync(int size)
        {
            var key = GetCountsKey();
            var list = new List<TEntity>(size);
            for (int i = 0; i < size; i++)
            {
                var ds = await Database.ListLeftPopAsync(key);
                if (!ds.HasValue)
                {
                    break;
                }
                var data = Deserialize(ds);
                list.Add(data);
            }
            if (list.Count != 0)
            {
                await DbContext.BulkInsertAsync(list);
            }
            return list.Count;
        }

        protected abstract Task<List<TStatistical>> GetStatisticalsAsync(StatisticLevels type, DateTime left, DateTime right);

        public virtual async Task<List<TStatistical>> SaveStatisticalAsync(StatisticLevels type, DateTime left, DateTime right)
        {
            var now = DateTime.Now;
            var data = await GetStatisticalsAsync(type,left, right);
            if (data.Count != 0)
            {
                await DbContext.BulkInsertAsync(data);
            }
            return data;
        }
    }
}
