using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using StackExchange.Redis;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using System.Collections.Generic;
using Anf.ResourceFetcher;

namespace Anf.Web
{
    internal partial class WebModuleEntry
    {
        public WebModuleEntry AddCache(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IDistributedCache, RedisCache>();
            services.AddOptions<RedisCacheOptions>()
                .Configure(x => x.Configuration = configuration["ConnectionStringsCacheConnection"]);
            services.AddMemoryCache();
            services.AddSingleton<IConnectionMultiplexer>(x =>
            {
                var c = x.GetRequiredService<IConfiguration>();
                var config = c["ConnectionStringsCacheConnection"];
                return ConnectionMultiplexer.Connect(config);
            });
            services.AddScoped(x => x.GetRequiredService<IConnectionMultiplexer>().GetDatabase());


            services.AddSingleton<IDistributedLockFactory>(x =>
            {
                var conn = x.GetRequiredService<IConnectionMultiplexer>();
                return RedLockFactory.Create(new List<RedLockMultiplexer> { new RedLockMultiplexer(conn) });
            });
            services.AddAutoCache();
            return this;
        }
    }
}
