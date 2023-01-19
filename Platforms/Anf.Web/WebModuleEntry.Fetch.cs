using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Anf.ResourceFetcher;

namespace Anf.Web
{
    internal partial class WebModuleEntry
    {
        public WebModuleEntry AddFetch(IServiceCollection services, IConfiguration configuration)
        {
            services.AddFetcherProvider()
               .AddDefaultFetcherProvider()
               .AddRedisFetcherProvider();
            services.AddResourceFetcher()
                .AddRedisResourceFetch();

            services.AddOptions<FetchOptions>();
            return this;
        }
    }
}
