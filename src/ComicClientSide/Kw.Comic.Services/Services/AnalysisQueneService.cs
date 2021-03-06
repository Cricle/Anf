using Kw.Comic.Engine;
using Kw.Comic.Services.Caches;
using Kw.Core.Annotations;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Services.Services
{
    [EnableService(ServiceLifetime = ServiceLifetime.Scoped)]
    internal class AnalysisService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly IDatabase redisDatabase;
        private readonly ComicEngine comicEngine;

        public async Task RasiseAsync()
        {
            var addr = await redisDatabase.ListRightPopAsync(ComicRedisKeys.AnalysisQueneKey);
            var str = addr.Get<string>();
            if (!string.IsNullOrEmpty(str))
            {
                var eng = comicEngine.GetComicSourceProviderType(str);
                if (eng == null)
                {
                    return;
                }
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var provider = (IComicSourceProvider)scope.ServiceProvider.GetRequiredService(eng.ProviderType);
                    var datas = await provider.GetChapterWithPageAsync(str);
                    var key = RedisKeyGenerator.Concat(ComicRedisKeys.ComicInfoKey, str);
                    
                }
            }
        }
    }
    [EnableService(ServiceLifetime = ServiceLifetime.Scoped)]
    internal class AnalysisQueneService
    {
        private readonly IDatabase redisDatabase;
        private readonly ComicEngine comicEngine;

        public AnalysisQueneService(IDatabase redisDatabase, ComicEngine comicEngine)
        {
            this.redisDatabase = redisDatabase;
            this.comicEngine = comicEngine;
        }

        public async Task<string[]> AllAsync()
        {
            var datas = await redisDatabase.SetMembersAsync(ComicRedisKeys.AnalysisQueneKey);
            return datas.ToStringArray();
        }
        public Task<bool> RemoveAsync(string address)
        {
            return redisDatabase.SetRemoveAsync(ComicRedisKeys.AnalysisQueneKey, address);
        }

        public Task ClearAsync()
        {
            return redisDatabase.KeyDeleteAsync(ComicRedisKeys.AnalysisQueneKey);
        }

        public async Task<string> RandomGetAsync()
        {
            var val = await redisDatabase.SetRandomMemberAsync(ComicRedisKeys.AnalysisQueneKey);
            return val.Get<string>();
        }

        public async Task<bool?> AddAsync(string address)
        {
            try
            {
                var t = comicEngine.GetComicSourceProviderType(address);
                if (t == null)
                {
                    return false;
                }
                //看下在不在
                var ok = await redisDatabase.SetAddAsync(ComicRedisKeys.AnalysisQueneKey, address);
                if (ok)
                {
                    return true;
                }
                return null;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
