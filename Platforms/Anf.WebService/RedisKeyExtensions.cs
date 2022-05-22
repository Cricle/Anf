using StackExchange.Redis;
using System.Collections.Generic;

namespace Anf.WebService
{
    public static class RedisKeyExtensions
    {
        public static async IAsyncEnumerable<string[]> SetScanAsync(this IDatabase db, string patter, int pageSize)
        {
            var count = 0L;
            do
            {
                var res =await db.ExecuteAsync("scan", count, "match", patter, "count", pageSize);
                var f = ((RedisResult[])res);
                count = ((long)f[0]);
                yield return ((string[])f[1]);
            } while (count != 0);
        }
    }
}
