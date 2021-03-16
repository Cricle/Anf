namespace Kw.Comic.Engine.Easy.Visiting
{
    public class GotChapterManagerInterceptorContext : ComicVisitingInterceptorContext
    {
        public IComicChapterManager ChapterManager { get; internal set; }
    }
}
