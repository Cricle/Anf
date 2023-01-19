namespace Anf.Easy.Visiting
{
    public class GotChapterManagerInterceptorContext<TResource> : ComicVisitingInterceptorContext<TResource>
    {
        public IComicChapterManager<TResource> ChapterManager { get; internal set; }
    }
}
