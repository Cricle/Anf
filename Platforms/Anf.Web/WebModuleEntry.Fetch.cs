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
        public WebModuleEntry AddFetch(IServiceCollection services, IConfiguration configuration)
        {
            services.AddFetcherProvider()
               .AddRedisFetcherProvider()
               .AddMssqlResourceFetcher()
               .AddDefaultFetcherProvider();
            services.AddResourceFetcher()
                .AddMssqlResourceFetcher()
                .AddRedisFetcherProvider();
            services.AddOptions<FetchOptions>();

            services.AddRedisResourceFetch();
            return this;
        }
    }
}
