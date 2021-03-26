using Kw.Comic.Engine.Easy;
using Kw.Comic.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Kw.Comic.Engine.Easy.Visiting;
using Xamarin.Forms;
using Kw.Comic.Engine.Easy.Store;

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
                            provider = easyComicBuilder.Build();
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

            Services.AddSingleton<IBookshelfService, BookshelfService>();
            Services.AddLogging();
            Services.AddSingleton<IComicVisiting<ImageSource>, ComicVisiting<ImageSource>>();
            Services.AddEasyComic(type);
            Services.AddDbContext<ComicDbContext>(x =>
            {
                var builder = new SqliteConnectionStringBuilder();
                builder.DataSource = XComicConst.DbFilePath;
                x.UseSqlite(builder.ConnectionString);
            },ServiceLifetime.Singleton);
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
