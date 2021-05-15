using Anf.ResourceFetcher;
using Anf.ResourceFetcher.Fetchers;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ResourceFetcherRegister
    {
        public static FetcherProvider AddFetcherProvider(this IServiceCollection services)
        {
            var provider = new FetcherProvider();
            services.AddSingleton(provider);
            return provider;
        }
        public static FetcherProvider AddDefaultFetcherProvider(this FetcherProvider provider)
        {
            provider.Add(typeof(RemoteFetcher));
            return provider;
        }
        public static IServiceCollection AddResourceFetcher(this IServiceCollection services)
        {
            services.AddScoped<RemoteFetcher>();
            services.AddScoped<IRootFetcher>(p =>
            {
                var lockFactor = p.GetRequiredService<IResourceLockerFactory>();
                var provider = p.GetRequiredService<FetcherProvider>();
                var rootFetcher = new RootFetcher(lockFactor);
                var fetchers = provider.Select(x => (IResourceFetcher)p.GetRequiredService(x)).ToArray();
                rootFetcher.AddRange(fetchers);
                return rootFetcher;
            });
            services.AddScoped<ISingleResourceFetcher>(x => x.GetRequiredService<IRootFetcher>());
            services.AddScoped<IBatchResourceFetcher>(x => x.GetRequiredService<IRootFetcher>());
            return services;
        }
    }
}
