using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
                .Configure(x => x.Configuration = configuration["ConnectionStrings:Redis"]);
            services.AddMemoryCache();
            services.AddSingleton<IConnectionMultiplexer>(x =>
            {
                var c = x.GetRequiredService<IConfiguration>();
                var config = c["ConnectionStrings:Redis"];
                return ConnectionMultiplexer.Connect(config);
            });
            services.AddScoped(x => x.GetRequiredService<IConnectionMultiplexer>().GetDatabase());


            services.AddSingleton<IDistributedLockFactory>(x =>
            {
                var conn = x.GetRequiredService<IConnectionMultiplexer>();
                return RedLockFactory.Create(new List<RedLockMultiplexer> { new RedLockMultiplexer(conn) });
            });
            return this;
        }
    }
}
