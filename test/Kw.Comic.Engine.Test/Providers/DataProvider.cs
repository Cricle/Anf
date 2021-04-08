using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Engine.Test.Providers
{
    internal class DataProvider : IComicSourceProvider
    {
        public Dictionary<string,ComicEntity> Entity { get; set; }

        public Dictionary<string, Func<Stream>> Stream { get; set; }

        public Dictionary<string, ComicPage[]> Pages { get; set; }
        public Task<ComicEntity> GetChaptersAsync(string targetUrl)
        {
            return Task.FromResult(Entity[targetUrl]);
        }

        public Task<Stream> GetImageStreamAsync(string targetUrl)
        {
            return Task.FromResult(Stream[targetUrl]());
        }

        public Task<ComicPage[]> GetPagesAsync(string targetUrl)
        {
            return Task.FromResult(Pages[targetUrl]);
        }
    }
}
