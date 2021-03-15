using System.IO;
using System.Threading;

namespace Kw.Comic.Engine.Easy
{
    public readonly struct ComicDownloadContext
    {
        public readonly ComicEntity Entity;
        public readonly ComicChapter Chapter;
        public readonly ComicPage Page;
        public readonly Stream SourceStream;
        public readonly CancellationToken Token;

        public ComicDownloadContext(ComicEntity entity,
            ComicChapter chapter,
            ComicPage page, 
            Stream sourceStream,
            CancellationToken token)
        {
            Token = token;
            Entity = entity;
            Chapter = chapter;
            Page = page;
            SourceStream = sourceStream;
        }
    }
}
