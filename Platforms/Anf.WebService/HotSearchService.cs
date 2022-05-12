using StackExchange.Redis;
using System.Threading.Tasks;

namespace Anf.WebService
{
    public class HotSearchService
    {
        private const string HotSearchKey = "Anf.WebService.HotSearchService.HotSearch";

        private readonly IDatabase redisDatabase;

        public HotSearchService(IDatabase redisDatabase)
        {
            this.redisDatabase = redisDatabase;
        }

        public Task<SortedSetEntry[]> GetHotSearchAsync(long skip, long take = 50, Order order = Order.Descending)
        {
            return redisDatabase.SortedSetRangeByRankWithScoresAsync(HotSearchKey, skip, take, order);
        }
        public Task<long> SizeAsync()
        {
            return redisDatabase.SortedSetLengthAsync(HotSearchKey);
        }

        public Task AddSearch(string keyword)
        {
            return redisDatabase.SortedSetAddAsync(HotSearchKey, keyword, 1);
        }
    }
}
