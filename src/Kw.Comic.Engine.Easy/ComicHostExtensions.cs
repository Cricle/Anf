using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy
{
    public static class ComicHostExtensions
    {
        public static IServiceScope GetServiceScope(this IComicHost host)
        {
            var factory = host.GetRequiredService<IServiceScopeFactory>();
            return factory.CreateScope();
        }
        public static ComicEngine GetComicEngine(this IComicHost host)
        {
            return host.GetRequiredService<ComicEngine>();
        }
        private static async Task<Tuple<IComicSourceProviderHost,ComicDownloadRequest, IComicDownloader>> MakeDownloadAsync(this IComicHost host,
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
            var dreq = new ComicDownloadRequest(saver, detail, provider);
            return new Tuple<IComicSourceProviderHost, ComicDownloadRequest, IComicDownloader>(
                provider, dreq, downloader);
        }
        public static async Task<bool> DownloadAsync(this IComicHost host,
            string address,
            IComicSaver saver,
            CancellationToken token=default)
        {
            var req =await host.MakeDownloadAsync(address, saver);
            if (req.Item1==null)
            {
                return false;
            }
            using (req.Item1)
            {
                await req.Item3.EmitAsync(req.Item2, token);
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
            if (req.Item1 == null)
            {
                return false;
            }
            using (req.Item1)
            {
                await req.Item3.BatchEmitAsync(req.Item2,concurrent, token);
                return true;
            }
        }
        public static IComicSourceProviderHost GetComicProvider(this IComicHost host, string address)
        {
            var eng = host.GetComicEngine();
            var type = eng.GetComicSourceProviderType(address);
            if (type==null)
            {
                return null;
            }
            var scope = host.GetServiceScope();
            var provider = (IComicSourceProvider)scope.ServiceProvider.GetRequiredService(type.ProviderType);
            return new ComicSourceProviderHost(provider, scope);
        }
        public static Task<ComicEntity> GetComicAsync(this IComicHost host, string address)
        {
            var provider = host.GetComicProvider(address);
            if (provider == null)
            {
                return null;
            }
            using (provider)
            {
                return provider.GetChaptersAsync(address);
            }
        }
        public static Task<ComicDetail> GetComicWithChaptersAsync(this IComicHost host, string address)
        {
            var provider = host.GetComicProvider(address);
            if (provider == null)
            {
                return null;
            }
            using (provider)
            {
                return provider.GetChapterWithPageAsync(address);
            }
        }
        public static Task<SearchComicResult> SearchAsync(this IComicHost host,string keywork,int skip,int take)
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
