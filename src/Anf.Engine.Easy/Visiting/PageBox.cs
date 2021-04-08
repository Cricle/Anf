using System;
using System.Threading.Tasks;

namespace Anf.Easy.Visiting
{
    public struct PageBox<TResource> : IComicVisitPage<TResource>,IDisposable
    {
        public bool DoNotDisposable { get; set; }

        public ComicPage Page { get; set; }

        public TResource Resource { get; set; }

        public void Dispose()
        {
            if (!DoNotDisposable&& Resource is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
