using Kw.Comic.Engine.Easy.Visiting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IO;

namespace Kw.Comic.Engine.Easy
{
    public static class ComicHostExtensions
    {
        public static IComicVisiting CreateVisiting(this IComicHost host)
        {
            return new ComicVisiting(host);
        }
        public static IServiceScope GetServiceScope(this IComicHost host)
        {
            var factory = host.GetRequiredService<IServiceScopeFactory>();
            return factory.CreateScope();
        }
        public static ComicEngine GetComicEngine(this IComicHost host)
        {
            return host.GetRequiredService<ComicEngine>();
        }
        public static async Task<DownloadLink> MakeDownloadAsync(this IComicHost host,
            string address,
            IComicSaver saver)
        {
            var provider = host.GetComicProvider(address);
            if (provider == null)
            {
                return default;
            }
            var detail = await provider.GetChapterWithPageAsync(address);
            if (detail == null)
            {
                return default;
            }
            var downloader = host.GetRequiredService<IComicDownloader>();
            var reqs = detail.Chapters.SelectMany(x => x.Pages.Select(y => new DownloadItemRequest(x.Chapter, y))).ToArray();
            var dreq = new ComicDownloadRequest(saver, detail.Entity, reqs, provider);
            return new DownloadLink(provider, downloader, dreq);
        }
        public static async Task<bool> DownloadAsync(this IComicHost host,
            string address,
            IComicSaver saver,
            CancellationToken token = default)
        {
            var req = await host.MakeDownloadAsync(address, saver);
            if (req.Downloader == null)
            {
                return false;
            }
            using (req.Host)
            {
                await req.Downloader.EmitAsync(req.Request, token);
                return true;
            }
        }
        public static async Task<bool> BatchDownloadAsync(this IComicHost host,
            string address,
            IComicSaver saver,
            int concurrent = 5,
            CancellationToken token = default)
        {
            var req = await host.MakeDownloadAsync(address, saver);
            if (req.Downloader == null)
            {
                return false;
            }
            using (req.Host)
            {
                await req.Downloader.BatchEmitAsync(req.Request, concurrent, token);
                return true;
            }
        }
        public static IComicSourceProviderHost GetComicProvider(this IComicHost host, string address)
        {
            var eng = host.GetComicEngine();
            var type = eng.GetComicSourceProviderType(address);
            if (type == null)
            {
                return null;
            }
            var scope = host.GetServiceScope();
            var provider = (IComicSourceProvider)scope.ServiceProvider.GetRequiredService(type.ProviderType);
            return new ComicSourceProviderHost(provider, scope);
        }
        public static async Task<ComicEntity> GetComicAsync(this IComicHost host, string address)
        {
            var provider = host.GetComicProvider(address);
            if (provider == null)
            {
                return null;
            }
            using (provider)
            {
                return await provider.GetChaptersAsync(address);
            }
        }
        public static async Task<ComicDetail> GetComicWithChaptersAsync(this IComicHost host, string address)
        {
            var provider = host.GetComicProvider(address);
            if (provider == null)
            {
                return null;
            }
            using (provider)
            {
                return await provider.GetChapterWithPageAsync(address);
            }
        }
        public static Task<SearchComicResult> SearchAsync(this IComicHost host, string keywork, int skip, int take)
        {
            var eng = host.GetSearchEngine();
            return eng.SearchAsync(keywork, skip, take);
        }
        public static SearchEngine GetSearchEngine(this IComicHost host)
        {
            return host.GetRequiredService<SearchEngine>();
        }
    }
}
