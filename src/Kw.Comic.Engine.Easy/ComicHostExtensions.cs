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
        public static async Task<IComicVisiting<T>> GetVisitingAndLoadAsync<T>(this IServiceProvider host, string address)
        {
            var visi = host.GetRequiredService<IComicVisiting<T>>();
            var r = await visi.LoadAsync(address);
            if (r)
            {
                return visi;
            }
            return null;
        }
        public static IComicVisiting<TResource> CreateVisiting<TResource>(this IServiceProvider host,
            IResourceFactoryCreator<TResource> resourceFactoryCreator)
        {
            return new ComicVisiting<TResource>(host, resourceFactoryCreator);
        }
        public static IComicVisiting<Stream> CreateVisiting(this IServiceProvider host,
            IResourceFactoryCreator<Stream> resourceFactoryCreator=null)
        {
            if (resourceFactoryCreator==null)
            {
                resourceFactoryCreator = StreamResourceFactoryCreator.Default;
            }
            return new ComicVisiting<Stream>(host, resourceFactoryCreator);
        }
        public static IServiceScope GetServiceScope(this IServiceProvider host)
        {
            var factory = host.GetRequiredService<IServiceScopeFactory>();
            return factory.CreateScope();
        }
        public static ComicEngine GetComicEngine(this IServiceProvider host)
        {
            return host.GetRequiredService<ComicEngine>();
        }
        public static DownloadLink LoadDownloadAsync(this IServiceProvider host,
            string address,
            ComicDetail detail,
            IComicSaver saver)
        {
            var provider = host.GetComicProvider(address);
            if (provider == null)
            {
                return default;
            }
            var downloader = host.GetRequiredService<IComicDownloader>();
            var reqs = detail.Chapters.SelectMany(x => x.Pages.Select(y => new DownloadItemRequest(x.Chapter, y))).ToArray();
            var dreq = new ComicDownloadRequest(saver, detail.Entity, detail, reqs, provider);
            return new DownloadLink(provider, downloader, dreq);
        }
        public static async Task<DownloadLink> MakeDownloadAsync(this IServiceProvider host,
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
            return LoadDownloadAsync(host, address, detail, saver);
        }
        public static async Task<bool> DownloadAsync(this IServiceProvider host,
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
        public static async Task<bool> BatchDownloadAsync(this IServiceProvider host,
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
        public static IComicSourceProviderHost GetComicProvider(this IServiceProvider host, string address)
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
        public static async Task<ComicEntity> GetComicAsync(this IServiceProvider host, string address)
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
        public static async Task<ComicDetail> GetComicWithChaptersAsync(this IServiceProvider host, string address)
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
        public static Task<SearchComicResult> SearchAsync(this IServiceProvider host, string keywork, int skip, int take)
        {
            var eng = host.GetSearchEngine();
            return eng.SearchAsync(keywork, skip, take);
        }
        public static SearchEngine GetSearchEngine(this IServiceProvider host)
        {
            return host.GetRequiredService<SearchEngine>();
        }
    }
}
