using System;
using System.Threading;

namespace Kw.Comic.Engine.Easy
{
    public class DownloadExceptionListenerContext : DownloadListenerContext
    {
        public DownloadExceptionListenerContext(ComicDownloadRequest request, ComicChapter chapter,
            ComicPage page, CancellationToken token,
            Exception exception)
            : base(request, chapter, page, token)
        {
            Exception = exception;
        }

        public Exception Exception { get; }
    }
}
