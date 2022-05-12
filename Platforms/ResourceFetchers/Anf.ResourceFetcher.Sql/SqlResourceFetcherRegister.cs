using Anf.ResourceFetcher.Fetchers;
using Microsoft.Extensions.DependencyInjection;

namespace Anf.ResourceFetcher
{
    public static class MssqlResourceFetcherRegister
    {
        public static FetcherProvider AddMssqlResourceFetcher(this FetcherProvider provider)
        {
            provider.Add(typeof(SqlFetcher));
            return provider;
        }
        public static IServiceCollection AddMssqlResourceFetcher(this IServiceCollection services)
        {
            services.AddScoped<SqlFetcher>();
            return services;
        }
    }
}
