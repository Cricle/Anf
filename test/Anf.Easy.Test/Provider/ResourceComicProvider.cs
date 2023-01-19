using Anf.Easy.Test.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Easy.Test.Provider
{
    internal class ResourceComicProvider : IComicSourceProvider
    {
        public Task<ComicEntity> GetChaptersAsync(string targetUrl)
        {
            return Task.FromResult(ResourceHelper.GetMonvzhilv());
        }

        public Task<Stream> GetImageStreamAsync(string targetUrl)
        {
            return Task.FromResult(Stream.Null);
        }

        public Task<ComicPage[]> GetPagesAsync(string targetUrl)
        {
            return Task.FromResult(ResourceHelper.GetMonvzhilvChatper0().Pages);
        }
    }
}
