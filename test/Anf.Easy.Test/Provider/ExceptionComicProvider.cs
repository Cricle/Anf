using System;
using System.IO;
using System.Threading.Tasks;

namespace Anf.Easy.Test.Provider
{
    internal class ExceptionComicProvider : IComicSourceProvider
    {
        public Task<ComicEntity> GetChaptersAsync(string targetUrl)
        {
            throw new Exception();
        }

        public Task<Stream> GetImageStreamAsync(string targetUrl)
        {
            throw new Exception();
        }

        public Task<ComicPage[]> GetPagesAsync(string targetUrl)
        {
            throw new Exception();
        }
    }
}
