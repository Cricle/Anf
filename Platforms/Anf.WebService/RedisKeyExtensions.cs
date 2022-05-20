using StackExchange.Redis;
using System.Collections.Generic;

namespace Anf.WebService
{
    /*
     * set->(阅读键s)
     * 使用时增加阅读键，任何定时同步
     */
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
