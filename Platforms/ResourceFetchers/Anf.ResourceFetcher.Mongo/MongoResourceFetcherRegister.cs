using Anf.ResourceFetcher.Fetchers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.ResourceFetcher
{
    public static class MongoResourceFetcherRegister
    {
        public static void AddMongo(this IServiceCollection services,string connectKey= "ConnectionStrings:MongoDb")
        {
            services.AddSingleton<IMongoClient>(p =>
            {
                var config = p.GetRequiredService<IConfiguration>();
                var settings = MongoClientSettings.FromConnectionString(config[connectKey]);
                return new MongoClient(settings);
            });
        }
        public static IServiceCollection AddMongoResourceFetcher(this IServiceCollection services)
        {
            services.AddScoped<MongoFetcher>();
            return services;
        }
        public static FetcherProvider AddMongoResourceFetcher(this FetcherProvider provider)
        {
            provider.Add(typeof(MongoFetcher));
            return provider;
        }
    }
}
