using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.WebService
{
    public class ComicRankService
    {
        private const string RankKey = "Anf.WebService.ComicRankService.Rank";

        private readonly IDatabase redisDatabase;

        public ComicRankService(IDatabase redisDatabase)
        {
            this.redisDatabase = redisDatabase;
        }

        public Task<SortedSetEntry[]> RangeAsync(int start = 0, int stop = -1, Order order = Order.Ascending)
        {
            return redisDatabase.SortedSetRangeByRankWithScoresAsync(RankKey, start, stop, order);
        }
        public Task<long> SizeAsync()
        {
            return redisDatabase.SortedSetLengthAsync(RankKey);
        }
        public async Task AddScopeAsync(string address)
        {
            await redisDatabase.SortedSetIncrementAsync(RankKey, address, 1);
        }
    }
}
