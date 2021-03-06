using System.IO;
using System.Threading;

namespace Kw.Comic.Engine.Easy
{
    public readonly struct ComicDownloadContext
    {
        public readonly ComicDetail ComicDetail;
        public readonly ChapterWithPage Chapter;
        public readonly ComicPage Page;
        public readonly Stream SourceStream;
        public readonly CancellationToken Token;

        public ComicDownloadContext(ComicDetail comicDetail,
            ChapterWithPage chapter, 
            ComicPage page, 
            Stream sourceStream,
            CancellationToken token)
        {
            Token = token;
            ComicDetail = comicDetail;
            Chapter = chapter;
            Page = page;
            SourceStream = sourceStream;
        }
    }
}
