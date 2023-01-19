using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Anf.AzureFunc;
using Anf.Engine;
using Anf.KnowEngines;
using Microsoft.IO;
using Anf.Networks;
using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.Jint;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Caching.Distributed;
using Anf.ResourceFetcher;
using Anf.ChannelModel.Entity;
using Anf.ResourceFetcher.Redis;
using Anf.ResourceFetcher.Fetchers;
using Anf.WebService;

namespace Anf.AzureFunc
{
    public static class Startup
    {
        public static void AddServices(IServiceCollection services, IConfiguration config)
        {
            services.AddKnowEngines();
            services.AddSingleton(x =>
            {
                var eng = new ComicEngine();
                eng.AddComicSource();
                return eng;
            });
            services.AddSingleton(x =>
            {
                var factory = x.GetRequiredService<IServiceScopeFactory>();
                var eng = new SearchEngine(factory);
                eng.AddSearchProvider();
                return eng;
            });
            services.AddSingleton(x =>
            {
                var factory = x.GetRequiredService<IServiceScopeFactory>();
                var eng = new ProposalEngine(factory);
                eng.AddProposalEngine();
                return eng;
            });
            var mgr = new RecyclableMemoryStreamManager();
            services.AddSingleton(mgr);
            services.AddHttpClient();
            services.AddScoped<INetworkAdapter, HttpClientAdapter>();
            services.AddScoped<IJsEngine, JintJsEngine>();
            services.AddScoped<IDistributedCache, RedisCache>();
            services.AddOptions<RedisCacheOptions>()
                .Configure(x => x.Configuration = config["ConnectionStrings:CacheConnection"]);
            services.AddDbContext<AnfDbContext>((x, y) =>
            {
                var config = x.GetRequiredService<IConfiguration>();
                y.UseSqlServer(config["ConnectionStrings:anfdb"]);
            }).AddIdentityCore<AnfUser>(x =>
            {
                x.Password.RequireDigit = false;
                x.Password.RequiredUniqueChars = 0;
                x.Password.RequireLowercase = false;
                x.Password.RequireNonAlphanumeric = false;
                x.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<AnfDbContext>();

            services.AddOptions<FetchOptions>();
            services.AddOptions<ResourceLockOptions>();
            services.AddFetcherProvider()
                .AddRedisFetcherProvider()
                .AddMssqlResourceFetcher()
                .AddDefaultFetcherProvider();
            services.AddResourceFetcher()
                .AddMssqlResourceFetcher()
                .AddRedisFetcherProvider();
            services.AddScoped<IDbContextTransfer, AnfDbContextTransfer>();
            services.AddRedis();
        }
    }
}
