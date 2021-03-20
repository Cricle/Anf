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
        public static IComicVisiting<string> CreateStoreVisitor(this IServiceProvider host)
        {
            return host.CreateVisiting(new StoreResourceCreatorFactory());
        }
        public static async Task<DownloadLink> CreateDownloadAsync(this IServiceProvider host, string address)
        {
            var storeService = host.GetRequiredService<IStoreService>();
            var lnk = await host.MakeDownloadAsync(address, new PhysicalComicSaver(storeService));
            return lnk;
        }
    }
}
