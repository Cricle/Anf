namespace Kw.Comic.Engine.Easy.Visiting
{
    public class GettingPageInterceptorContext : GotChapterManagerInterceptorContext
    {
        public int Index { get; internal set; }

        public ComicPage Page { get; internal set; }
    }
}
