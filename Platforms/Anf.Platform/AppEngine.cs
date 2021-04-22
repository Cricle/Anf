using Anf.Easy;
using Anf.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
#if EnableBookshelfService
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
#endif
using Anf.Easy.Visiting;
using Anf.Easy.Store;
using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.Jint;
using Anf.KnowEngines;
using Microsoft.Extensions.Logging;

namespace Anf
{
    public static class AppEngine
    {
        private static readonly object locker = new object();
        private static IServiceProvider provider;
        private static EasyComicBuilder easyComicBuilder;

        public static IServiceCollection Services => easyComicBuilder?.Services;
        public static bool IsLoaded => !(provider is null);

        public static IServiceProvider Provider
        {
            get
            {
                if (provider == null)
                {
                    lock (locker)
                    {
                        if (provider == null)
                        {
                            provider = easyComicBuilder.Build();
                            provider.UseKnowEngines();
                        }
                    }
                }
                return provider;
            }
        }
        public static void AddServices(NetworkAdapterTypes type= NetworkAdapterTypes.HttpClient)
        {
            Services.AddLogging();
            Services.AddEasyComic(type);
            Services.AddKnowEngines();
            Services.AddScoped<IJsEngine, JintJsEngine>();
        }
        public static void Reset(IServiceCollection services = null)
        {
            easyComicBuilder = new EasyComicBuilder(services);
            provider = null;
        }
        public static object GetService(Type type)
        {
            return Provider.GetService(type);
        }
        public static T GetService<T>()
        {
            return Provider.GetService<T>();
        }
        public static T GetRequiredService<T>()
        {
            return Provider.GetRequiredService<T>();
        }
        public static ILogger<T> GetLogger<T>()
        {
            return Provider.GetRequiredService<ILoggerFactory>().CreateLogger<T>();
        }
        public static object GetRequiredService(Type type)
        {
            return Provider.GetRequiredService(type);
        }
        public static IServiceScope CreateScope()
        {
            return Provider.GetRequiredService<IServiceScopeFactory>().CreateScope();
        }
    }
}
