namespace Anf.Easy.Visiting
{
    public class LoadVisitingInterceptorContext<TResource> : ComicVisitingInterceptorContext<TResource>
    {
        public bool IsSwitch { get; internal set; }

        public string Address { get; internal set; }
    }
}
