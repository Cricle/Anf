using Anf.ChannelModel.Entity;
using Anf.ResourceFetcher.Fetchers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

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
