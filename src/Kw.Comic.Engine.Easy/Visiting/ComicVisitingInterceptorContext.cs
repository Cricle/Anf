namespace Kw.Comic.Engine.Easy.Visiting
{
    public class ComicVisitingInterceptorContext<TResource>
    {
        public IComicVisiting<TResource> Visiting { get; internal set; }
    }
}
