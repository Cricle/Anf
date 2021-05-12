using Anf.ChannelModel;
using Anf.ChannelModel.KeyGenerator;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Anf.Web.Services
{
    public class UserIdentityService
    {
        private static readonly string UserMapKey = "Anf.Web.Services.UserIdentityService.UserMap";
        private readonly IDatabase database;

        public static readonly TimeSpan ExpireTime = TimeSpan.FromDays(6);

        public UserIdentityService(IDatabase database)
        {
            this.database = database;
        }

        public async Task<UserSnapshot> GetTokenInfoAsync(string token)
        {
            var key = RedisKeyGenerator.Concat(UserMapKey, token);
            var val = await database.StringGetAsync(key);
            return val.Get<UserSnapshot>();
        }
        public async Task<string> SetIdentityAsync(UserSnapshot snapshot)
        {
            var tk = Guid.NewGuid().ToString();
            var key = RedisKeyGenerator.Concat(UserMapKey, tk);
            var bytes = JsonSerializer.SerializeToUtf8Bytes(snapshot);
            await database.StringSetAsync(key, bytes, ExpireTime);
            return tk;
        }
    }
}
