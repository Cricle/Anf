using System.IO;
using System.Threading.Tasks;

namespace Anf.Easy.Test
{
    class ComicSourceProvider : IComicSourceProvider
    {
        public Task<ComicEntity> GetChaptersAsync(string targetUrl)
        {
            return Task.FromResult<ComicEntity>(new ComicEntity());
        }

        public Task<Stream> GetImageStreamAsync(string targetUrl)
        {
            return Task.FromResult<Stream>(new MemoryStream());
        }

        public Task<ComicPage[]> GetPagesAsync(string targetUrl)
        {
            return Task.FromResult(new ComicPage[0]);
        }
    }
}
