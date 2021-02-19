using Kw.Comic.Engine;

namespace Kw.Comic.Visit
{
    public readonly struct ResolvedChapterContext
    {
        public readonly int Total;

        public readonly int Current;

        public readonly ComicVisitor ComicVisitor;

        public readonly ComicEntity ComicEntity;

        public ResolvedChapterContext(int total, int current, ComicVisitor comicVisitor, ComicEntity comicEntity)
        {
            Total = total;
            Current = current;
            ComicVisitor = comicVisitor;
            ComicEntity = comicEntity;
        }
    }
}
