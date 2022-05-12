using System;

namespace Anf.ResourceFetcher
{
    public interface IResourceLocker : IDisposable
    {
        string Resource { get; }
        bool IsAcquired { get; }
    }
}
