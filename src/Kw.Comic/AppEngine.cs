using Kw.Comic.Engine.Easy;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;

namespace Kw.Comic
{
    public static class AppEngine
    {
        private static readonly object locker = new object();
        private static IServiceProvider provider;
        private static EasyComicBuilder easyComicBuilder;

        public static IServiceCollection Services => easyComicBuilder?.Services;

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
                            provider = easyComicBuilder.Services.BuildServiceProvider();
                        }
                    }
                }
                return provider;
            }
        }
        public static void AddDefaultsServices(string storePath)
        {
            if (Services==null)
            {
                Reset();
            }
            Debug.Assert(Services != null);
            Services.AddCoreServices(storePath);

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
