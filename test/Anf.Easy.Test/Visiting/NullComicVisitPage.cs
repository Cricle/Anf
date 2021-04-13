using Anf.Easy.Visiting;

namespace Anf.Easy.Test.Visiting
{
    internal class NullComicVisitPage<T> : IComicVisitPage<T>
    {
        public ComicPage Page { get; set; }

        public T Resource { get; set; }
    }
}
