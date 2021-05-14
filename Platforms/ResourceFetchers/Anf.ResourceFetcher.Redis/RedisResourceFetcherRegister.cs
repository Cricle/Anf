using Anf.ResourceFetcher.Fetchers;
using Anf.ResourceFetcher.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.ResourceFetcher
{
    public static class RedisResourceFetcherRegister
    {
        public static FetcherProvider AddRedisFetcherProvider(this FetcherProvider provider)
        {
            provider.Add(typeof(RedisFetcher));
            return provider;
        }
        public static IServiceCollection AddRedisFetcherProvider(this IServiceCollection services)
        {
            services.AddScoped<RedisFetcher>();
            return services;
        }
        public static IServiceCollection AddRedis(this IServiceCollection services)
        {
            services.AddSingleton<IConnectionMultiplexer>(x =>
            {
                var c = x.GetRequiredService<IConfiguration>();
                var config = c["ConnectionStrings:CacheConnection"];
                return ConnectionMultiplexer.Connect(config);
            });
            services.AddScoped(x => x.GetRequiredService<IConnectionMultiplexer>().GetDatabase());


            services.AddSingleton<IDistributedLockFactory>(x =>
            {
                var conn = x.GetRequiredService<IConnectionMultiplexer>();
                return RedLockFactory.Create(new List<RedLockMultiplexer> { new RedLockMultiplexer(conn) });
            });
            services.AddSingleton<IResourceLockerFactory, ResourceLockerFactory>();
            services.AddScoped<InRedisComicService>();
            return services;
        }
    }
}
