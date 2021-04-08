namespace Anf.Easy.Visiting
{
    public class ChapterVisitingInterceptorContext<TResource> : ComicVisitingInterceptorContext<TResource>
    {
        public ChapterWithPage Chapter { get; internal set; }
    }
    public class ChapteringVisitingInterceptorContext<TResource> : ComicVisitingInterceptorContext<TResource>
    {
        public int Index { get; internal set; }

        public ComicChapter Chapter { get; internal set; }
    }
}
