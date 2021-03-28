﻿using Kw.Comic.Engine.Easy;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Consolat
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
