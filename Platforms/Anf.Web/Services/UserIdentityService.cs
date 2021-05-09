using Anf.ChannelModel.KeyGenerator;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anf.Web.Services
{
    public class UserIdentityService
    {
        private static readonly string UserMapKey = "Anf.Web.Services.UserIdentityService.UserMap";
        private readonly IDatabase database;

        public static readonly TimeSpan ExpireTime = TimeSpan.FromDays(6);

        public async Task<string> SetIdentityAsync(string connectId,string userName)
        {
            var tk = Guid.NewGuid().ToString();
            var key = RedisKeyGenerator.Concat(UserMapKey, connectId,userName);
            await database.StringSetAsync(key, tk, ExpireTime);
            return tk;
        }
    }
}
