using Anf.Easy;
using Microsoft.Extensions.DependencyInjection;
using System;
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
        public static bool IsLoaded => provider != null;

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
                        }
                    }
                }
                return provider;
            }
        }
        public static void UseProvider(IServiceProvider provider)
        {
            AppEngine.provider = provider;
        }
        public static void AddServices(NetworkAdapterTypes type= NetworkAdapterTypes.HttpClient)
        {
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
