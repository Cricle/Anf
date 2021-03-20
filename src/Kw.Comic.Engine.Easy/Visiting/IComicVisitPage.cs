using System.IO;

namespace Kw.Comic.Engine.Easy.Visiting
{
    public interface IComicVisitPage<TResource>
    {
        ComicPage Page { get; }

        TResource Resource { get; }
    }
}
