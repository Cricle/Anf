using Kw.Comic.Engine;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kw.Comic.Web.Services
{
    public class ComicDetailCacher
    {
        public static readonly string Head = "Kw.Comic.Web.Services.ComicDetailCacher";
        public static readonly string EntityHead = "Kw.Comic.Web.Services.EntityCacher";

        public static readonly TimeSpan CacheTime = TimeSpan.FromHours(1);
        public const string DefaultSplit = "_";

        private readonly IMemoryCache memoryCache;

        public ComicDetailCacher(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }
        private static string GetDetailKey(string address)
        {
            return string.Concat(Head, DefaultSplit, address);
        }
        private static string GetEntityKey(string address)
        {
            return string.Concat(EntityHead, DefaultSplit, address);
        }

        public ComicDetail GetDetail(string address)
        {
            var key = GetDetailKey(address);
            return memoryCache.Get<ComicDetail>(key);
        }
        public ComicEntity GetEntity(string address)
        {
            var key = GetEntityKey(address);
            return memoryCache.Get<ComicEntity>(key);
        }
        public void SetDetail(string address, ComicDetail detail)
        {
            var key = GetDetailKey(address);
            memoryCache.Set(key, detail, CacheTime);
            SetEntity(address, detail.Entity);
        }
        public void SetEntity(string address,ComicEntity entity)
        {
            var key = GetEntityKey(address);
            memoryCache.Set(key,entity,CacheTime);
        }
    }
}
