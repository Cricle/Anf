using Anf.ChannelModel.Entity;
using Anf.ResourceFetcher;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Anf.ResourceFetcher.Fetchers;
using RedLockNet;
using System.Linq;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using Anf.ResourceFetcher.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ResourceFetcherRegister
    {
        public static void AddDefaultFetcher(this IServiceCollection services)
        {
            var provider = FetcherProvider.CreateDefault();
            foreach (var item in provider)
            {
                services.AddScoped(item);
            }
            services.AddSingleton(provider);
        }
        public static void AddResourceFetcher(this IServiceCollection services)
        {
            services.AddSingleton<IConnectionMultiplexer>(x =>
            {
                var c = x.GetRequiredService<IConfiguration>();
                var config = c["ConnectionStrings:CacheConnection"];
                return ConnectionMultiplexer.Connect(config);
            });
            services.AddScoped(x => x.GetRequiredService<IConnectionMultiplexer>().GetDatabase());

            services.AddSingleton<IMongoClient>(p=> 
            {
                var config = p.GetRequiredService<IConfiguration>();
                var settings = MongoClientSettings.FromConnectionString(config["ConnectionStrings:MongoDb"]);
                return new MongoClient(settings);
            });
            services.AddSingleton<IDistributedLockFactory>(x =>
            {
                var conn = x.GetRequiredService<IConnectionMultiplexer>();
                return RedLockFactory.Create(new List<RedLockMultiplexer> { new RedLockMultiplexer(conn) });
            });
            services.AddScoped<IRootFetcher>(p =>
            {
                var lockFactor = p.GetRequiredService<IDistributedLockFactory>();
                var provider = p.GetRequiredService<FetcherProvider>();
                var rootFetcher = new RootFetcher(lockFactor);
                var fetchers = provider.Select(x => (IResourceFetcher)p.GetRequiredService(x)).ToArray();
                rootFetcher.AddRange(fetchers);
                return rootFetcher;
            });
            services.AddScoped<InRedisComicService>();
        }
    }
}
