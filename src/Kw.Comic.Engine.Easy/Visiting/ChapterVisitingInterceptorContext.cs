namespace Kw.Comic.Engine.Easy.Visiting
{
    public class ChapterVisitingInterceptorContext : ComicVisitingInterceptorContext
    {
        public ChapterWithPage Chapter { get; internal set; }
    }
    public class ChapteringVisitingInterceptorContext : ComicVisitingInterceptorContext
    {
        public int Index { get; internal set; }

        public ComicChapter Chapter { get; internal set; }
    }
}
