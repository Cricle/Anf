using System.IO;
using System.Threading.Tasks;

namespace Anf.Easy.Test.Visiting
{
    internal class NullSourceProvider : IComicSourceProvider
    {
        public bool GetChaptersAsyncVal { get; set; }
        public bool GetImageStreamAsyncVal { get; set; }
        public bool GetPagesAsyncVal { get; set; }


        public async Task<ComicEntity> GetChaptersAsync(string targetUrl)
        {
            GetChaptersAsyncVal = true;
            return null;
        }

        public async Task<Stream> GetImageStreamAsync(string targetUrl)
        {
            GetImageStreamAsyncVal = true;
            return null;
        }

        public async Task<ComicPage[]> GetPagesAsync(string targetUrl)
        {
            GetPagesAsyncVal = true;
            return null;
        }
    }
}
