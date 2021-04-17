using System.Threading;

namespace Anf.Easy
{
    public class DownloadListenerContext
    {
        public DownloadListenerContext(ComicDownloadRequest request, ComicChapter chapter, ComicPage page, CancellationToken token)
        {
            Request = request;
            Chapter = chapter;
            Page = page;
            Token = token;
        }

        public ComicDownloadRequest Request { get; }
        
        public ComicChapter Chapter { get; }
        
        public ComicPage Page { get; }

        public CancellationToken Token { get; }
    }
}
