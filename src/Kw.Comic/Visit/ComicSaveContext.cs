using Kw.Comic.Engine;

namespace Kw.Comic.Visit
{
    public readonly struct ComicSaveContext
    {
        public readonly int Total;

        public readonly int Current;

        public readonly ChapterWithPage ChapterWithPage;

        public readonly ComicEntity ComicEntity;

        public readonly ChapterVisitorBase ChapterVisitor;

        public ComicSaveContext(int total,
            int current,
            ChapterWithPage chapterWithPage, 
            ComicEntity comicEntity, 
            ChapterVisitorBase chapterVisitor)
        {
            Total = total;
            Current = current;
            ChapterWithPage = chapterWithPage;
            ComicEntity = comicEntity;
            ChapterVisitor = chapterVisitor;
        }
    }
}
