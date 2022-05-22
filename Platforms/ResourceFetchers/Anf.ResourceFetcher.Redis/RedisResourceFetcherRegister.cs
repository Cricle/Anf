using Anf.ResourceFetcher.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;
using System.Collections.Generic;

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
        public static IServiceCollection AddRedisResourceFetch(this IServiceCollection services)
        {
            services.AddSingleton<IResourceLockerFactory, ResourceLockerFactory>();
            services.AddScoped<InRedisComicService>();
            return services;
        }
    }
}
