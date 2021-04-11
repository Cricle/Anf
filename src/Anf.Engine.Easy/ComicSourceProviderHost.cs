using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Threading.Tasks;

namespace Anf.Easy
{
    internal class ComicSourceProviderHost : IComicSourceProviderHost
    {
        public IComicSourceProvider ComicSourceProvider { get; }
        public IServiceScope Scope { get; }
        public ComicSourceProviderHost(IComicSourceProvider comicSourceProvider,IServiceScope scope)
        {
            ComicSourceProvider = comicSourceProvider;
            Scope = scope;
        }

        public void Dispose()
        {
            Scope?.Dispose();
        }

        public Task<ComicEntity> GetChaptersAsync(string targetUrl)
        {
            return ComicSourceProvider.GetChaptersAsync(targetUrl);
        }

        public Task<Stream> GetImageStreamAsync(string targetUrl)
        {
            return ComicSourceProvider.GetImageStreamAsync(targetUrl);
        }

        public Task<ComicPage[]> GetPagesAsync(string targetUrl)
        {
            return ComicSourceProvider.GetPagesAsync(targetUrl);
        }
    }
}
