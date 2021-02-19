using System;
using System.Threading.Tasks;

namespace Kw.Comic.Visit
{
    public interface IResourceVisitor : IDisposable
    {
        bool IsLoaded { get; }

        Task LoadAsync();
    }
}
