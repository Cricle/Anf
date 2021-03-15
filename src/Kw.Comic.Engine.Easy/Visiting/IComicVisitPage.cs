using System.IO;

namespace Kw.Comic.Engine.Easy.Visiting
{
    public interface IComicVisitPage
    {
        ComicPage Page { get; }

        Stream Stream { get; }
    }
}
