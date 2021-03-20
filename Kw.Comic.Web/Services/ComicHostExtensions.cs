using Kw.Comic.Engine.Easy;
using Kw.Comic.Engine.Easy.Visiting;
using KwC.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KwC.Services
{
    public static class ComicHostExtensions
    {
        private static readonly LruCacher<string, DownloadLink> downloadCache=new LruCacher<string, DownloadLink>(10);
        public static IComicVisiting<string> CreateStoreVisitor(this IServiceProvider host)
        {
            return host.CreateVisiting(new StoreResourceCreatorFactory());
        }
        public static async Task<DownloadLink> CreateDownloadAsync(this IServiceProvider host, string address)
        {
            if (downloadCache.ContainsKey(address))
            {
                return downloadCache.Get(address);
            }
            var storeService = host.GetRequiredService<IStoreService>();
            var lnk = await host.MakeDownloadAsync(address, new PhysicalComicSaver(storeService));
            downloadCache.Add(address, lnk);
            return lnk;
        }
    }
}
