using StackExchange.Redis;
using System.Threading.Tasks;

namespace Anf.WebService
{
    public class ComicRankService
    {
        private static readonly RedisKey RankKey = "Anf.WebService.ComicRankService.Visit";
        private static readonly RedisKey SearchKey = "Anf.WebService.ComicRankService.Search";

        private readonly IDatabase redisDatabase;

        public ComicRankService(IDatabase redisDatabase)
        {
            this.redisDatabase = redisDatabase;
        }

        public Task<SortedSetEntry[]> RangeVisitAsync(int start = 0, int stop = -1, Order order = Order.Descending)
        {
            return redisDatabase.SortedSetRangeByRankWithScoresAsync(RankKey, start, stop, order);
        }
        public Task<SortedSetEntry[]> RangeSearchAsync(int start = 0, int stop = -1, Order order = Order.Descending)
        {
            return redisDatabase.SortedSetRangeByRankWithScoresAsync(SearchKey, start, stop, order);
        }
        public Task<long> SizeAsync()
        {
            return redisDatabase.SortedSetLengthAsync(RankKey);
        }
        public async Task IncVisitAsync(string address,int scope)
        {
            await redisDatabase.SortedSetIncrementAsync(RankKey, address, scope);
        }
        public async Task IncSearchAsync(string content, int scope)
        {
            await redisDatabase.SortedSetIncrementAsync(SearchKey, content, scope);
        }
    }
}
