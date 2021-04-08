using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Anf
{
    public interface IComicSourceProvider
    {
        Task<Stream> GetImageStreamAsync(string targetUrl);

        Task<ComicEntity> GetChaptersAsync(string targetUrl);

        Task<ComicPage[]> GetPagesAsync(string targetUrl);
    }
}
