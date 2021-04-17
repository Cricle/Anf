using System.IO;

namespace Anf.Easy.Visiting
{
    public interface IComicVisitPage<TResource>
    {
        ComicPage Page { get; }

        TResource Resource { get; }
    }
}
