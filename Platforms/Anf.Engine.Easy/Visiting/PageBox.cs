using System;
using System.Threading.Tasks;

namespace Anf.Easy.Visiting
{
    public struct PageBox<TResource> : IComicVisitPage<TResource>,IDisposable
    {
        public bool DoNotDispose { get; set; }

        public ComicPage Page { get; set; }

        public TResource Resource { get; set; }

        public void Dispose()
        {
            if (!DoNotDispose&& Resource is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
