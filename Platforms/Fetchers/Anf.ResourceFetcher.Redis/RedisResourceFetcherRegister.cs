using Anf.ResourceFetcher.Redis;
using Microsoft.Extensions.DependencyInjection;

namespace Anf.ResourceFetcher
{
    public static class RedisResourceFetcherRegister
    {
        public static FetcherProvider AddRedisFetcherProvider(this FetcherProvider provider)
        {
            provider.Add(typeof(RedisFetcher));
            return provider;
        }
        public static IServiceCollection AddRedisResourceFetch(this IServiceCollection services)
        {
            services.AddScoped<RedisFetcher>();
            services.AddSingleton<IResourceLockerFactory, ResourceLockerFactory>();
            services.AddScoped<InRedisComicService>();
            return services;
        }
    }
}
