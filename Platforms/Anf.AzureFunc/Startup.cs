using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Anf.AzureFunc;
using Anf.Engine;
using Anf.KnowEngines;
using Microsoft.IO;
using Anf.Networks;
using System.IO;
using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.Jint;
using Microsoft.Extensions.Configuration;
using Azure.Identity;
using StackExchange.Redis;
using Anf.AzureFunc.Services;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Caching.Distributed;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Anf.AzureFunc
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var services = builder.Services;
            services.AddKnowEngines();
            services.AddSingleton(x=> 
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
            services.AddSingleton(x=> 
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
            var ctx = builder.GetContext();
            services.AddScoped<IDistributedCache, RedisCache>();
            services.AddOptions<RedisCacheOptions>()
                .Configure(x => x.Configuration = ctx.Configuration["ConnectionStrings:CacheConnection"]);
            var conn = ConnectionMultiplexer.Connect(ctx.Configuration["ConnectionStrings:CacheConnection"]);
            services.AddSingleton<IConnectionMultiplexer>(conn);
            services.AddScoped(x => x.GetRequiredService<IConnectionMultiplexer>().GetDatabase());
            services.AddScoped<AnalysisService>();
        }
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            base.ConfigureAppConfiguration(builder);
            //var keyVaultEndpoint = new Uri(Environment.GetEnvironmentVariable("VaultUri"));
            //builder.ConfigurationBuilder.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());
        }
    }
}
