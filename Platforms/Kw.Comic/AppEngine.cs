using Kw.Comic.Engine.Easy;
using Kw.Comic.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
#if EnableBookshelfService
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
#endif
using Kw.Comic.Engine.Easy.Visiting;
using Kw.Comic.Engine.Easy.Store;
using JavaScriptEngineSwitcher.Core;
using JavaScriptEngineSwitcher.Jint;

namespace Kw.Comic
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
            XComicConst.EnsureDataFolderCreated();

            var store = FileStoreService.FromMd5Default(XComicConst.CacheFolderPath);
#if EnableBookshelfService
            Services.AddSingleton<IBookshelfService, BookshelfService>();
            Services.AddDbContext<ComicDbContext>(x =>
            {
                var builder = new SqliteConnectionStringBuilder();
                builder.DataSource = XComicConst.DbFilePath;
                x.UseSqlite(builder.ConnectionString);
            },ServiceLifetime.Singleton);
#endif
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
