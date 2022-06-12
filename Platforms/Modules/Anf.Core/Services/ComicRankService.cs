using Anf.ChannelModel.Mongo;
using Anf.ResourceFetcher.Fetchers;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Anf.WebService
{
    public class ComicRankService
    {
        private static readonly RedisKey RankKey = "Anf.WebService.ComicRankService.Visit";
        private static readonly RedisKey SearchKey = "Anf.WebService.ComicRankService.Search";

        private readonly IDatabase redisDatabase;
        private readonly RootFetcher rootFetcher;

        public ComicRankService(IDatabase redisDatabase,
            RootFetcher rootFetcher)
        {
            this.redisDatabase = redisDatabase;
            this.rootFetcher = rootFetcher;
        }
        public async Task<AnfComicEntityScoreTruck[]> RangeVisitEntityAsync(int start = 0, int stop = -1, Order order = Order.Descending)
        {
            var data = await RangeVisitAsync(start, stop, order);
            var identity = new FetchChapterIdentity[data.Length];
            var scoreMap = new Dictionary<string, double>(data.Length);
            for (int i = 0; i < data.Length; i++)
            {
                var eleStr = data[i].Element.ToString();
                scoreMap[eleStr] = data[i].Score;
                identity[i] = new FetchChapterIdentity(eleStr, eleStr);
            }
            var truck = await rootFetcher.FetchEntitysAsync(identity);
            var res = new AnfComicEntityScoreTruck[truck.Length];
            for (int i = 0; i < truck.Length; i++)
            {
                var t = truck[i];
                var ti = new AnfComicEntityScoreTruck { Truck = t };
                if (scoreMap.TryGetValue(t.ComicUrl,out var s))
                {
                    ti.Score = s;
                }
                res[i] = ti;
            }
            return res;
        }
        public Task<SortedSetEntry[]> RangeVisitAsync(int start = 0, int stop = -1, Order order = Order.Descending)
        {
            return redisDatabase.SortedSetRangeByRankWithScoresAsync(RankKey, start, stop, order);
        }
        public Task<SortedSetEntry[]> RangeSearchAsync(int start = 0, int stop = -1, Order order = Order.Descending)
        {
            return redisDatabase.SortedSetRangeByRankWithScoresAsync(SearchKey, start, stop, order);
        }
        public Task<long> SizeVisitAsync()
        {
            return redisDatabase.SortedSetLengthAsync(RankKey);
        }
        public Task<long> SizeSearchAsync()
        {
            return redisDatabase.SortedSetLengthAsync(SearchKey);
        }
        public async Task IncVisitAsync(string address, int scope)
        {
            await redisDatabase.SortedSetIncrementAsync(RankKey, address, scope);
        }
        public async Task IncSearchAsync(string content, int scope)
        {
            await redisDatabase.SortedSetIncrementAsync(SearchKey, content, scope);
        }
    }
}
