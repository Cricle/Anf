using Anf.ChannelModel;
using SecurityLogin;
using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Anf.WebService
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
            var key = KeyGenerator.Concat(UserMapKey, token);
            var val = await database.StringGetAsync(key);
            return val.Get<UserSnapshot>();
        }
        public async Task<string> SetIdentityAsync(UserSnapshot snapshot)
        {
            var tk = Guid.NewGuid().ToString();
            var key = KeyGenerator.Concat(UserMapKey, tk);
            var bytes = JsonSerializer.SerializeToUtf8Bytes(snapshot);
            await database.StringSetAsync(key, bytes, ExpireTime);
            return tk;
        }
    }
}
