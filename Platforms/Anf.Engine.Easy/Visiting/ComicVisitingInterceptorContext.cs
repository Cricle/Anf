namespace Anf.Easy.Visiting
{
    public class ComicVisitingInterceptorContext<TResource>
    {
        public IComicVisiting<TResource> Visiting { get; internal set; }
    }
}
