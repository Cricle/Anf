using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.ResourceFetcher
{
    public interface IResourceLocker : IDisposable
    {
        string Resource { get; }
        bool IsAcquired { get; }
    }
}
