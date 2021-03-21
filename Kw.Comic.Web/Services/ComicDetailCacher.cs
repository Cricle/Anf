using Kw.Comic.Engine;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kw.Comic.Web.Services
{
    public class ComicDetailCacher
    {
        public static readonly string Head = "Kw.Comic.Web.Services.ComicDetailCacher";
        public static readonly string EntityHead = "Kw.Comic.Web.Services.EntityCacher";

        public static readonly TimeSpan CacheTime = TimeSpan.FromHours(1);
        public const string DefaultSplit = "_";
        public const string DefaultCacheFolder = "ComicCache";

        public DirectoryInfo CacheFolder { get; }

        private readonly IMemoryCache memoryCache;

        public ComicDetailCacher(IMemoryCache memoryCache,
            IHostEnvironment host)
        {
            this.memoryCache = memoryCache;
            CacheFolder = new DirectoryInfo(Path.Combine(host.ContentRootPath, DefaultCacheFolder));
            if (!CacheFolder.Exists)
            {
                CacheFolder.Create();
            }
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
            var val = GetDetailFromFile(address);
            if (val!=null)
            {
                return val;
            }
            return memoryCache.Get<ComicDetail>(key);
        }
        public ComicEntity GetEntity(string address)
        {
            var detail = GetDetail(address);
            if (detail!=null)
            {
                return detail.Entity;
            }
            var key = GetEntityKey(address);
            return memoryCache.Get<ComicEntity>(key);
        }
        public void SetDetail(string address, ComicDetail detail)
        {
            var key = GetDetailKey(address);
            memoryCache.Set(key, detail, CacheTime);
            SetEntity(address, detail.Entity);
            SetDetailToFile(address, detail);
        }
        public void SetEntity(string address,ComicEntity entity)
        {
            var key = GetEntityKey(address);
            memoryCache.Set(key,entity,CacheTime);
        }
        private void SetDetailToFile(string address,ComicDetail detail)
        {
            var key = Md5Helper.MakeMd5(address);
            var str = JsonSerializer.Serialize(detail);
            File.WriteAllText(Path.Combine(CacheFolder.FullName, key), str);
        }
        private ComicDetail GetDetailFromFile(string address)
        {
            var key = Md5Helper.MakeMd5(address);
            var fi = CacheFolder.EnumerateFiles(key, SearchOption.TopDirectoryOnly).FirstOrDefault();
            if (fi==null)
            {
                return null;
            }
            var content = File.ReadAllText(fi.FullName);
            return JsonSerializer.Deserialize<ComicDetail>(content);
        }
    }
}
