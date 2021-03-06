using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy
{
    internal class ComicSourceProviderHost : IComicSourceProviderHost
    {
        private readonly IComicSourceProvider comicSourceProvider;
        private readonly IServiceScope scope;

        public ComicSourceProviderHost(IComicSourceProvider comicSourceProvider, IServiceScope scope)
        {
            this.comicSourceProvider = comicSourceProvider;
            this.scope = scope;
        }

        public void Dispose()
        {
            scope.Dispose();
        }

        public Task<ComicEntity> GetChaptersAsync(string targetUrl)
        {
            return comicSourceProvider.GetChaptersAsync(targetUrl);
        }

        public Task<Stream> GetImageStreamAsync(string targetUrl)
        {
            return comicSourceProvider.GetImageStreamAsync(targetUrl);
        }

        public Task<ComicPage[]> GetPagesAsync(string targetUrl)
        {
            return comicSourceProvider.GetPagesAsync(targetUrl);
        }
    }
}
