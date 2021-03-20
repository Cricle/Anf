namespace Kw.Comic.Engine.Easy.Visiting
{
    public struct PageBox<TResource> : IComicVisitPage<TResource>
    {
        public ComicPage Page { get; set; }

        public TResource Resource { get; set; }
    }

}
