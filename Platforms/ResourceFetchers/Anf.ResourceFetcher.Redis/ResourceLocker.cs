using RedLockNet;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.ResourceFetcher.Redis
{
    internal class ResourceLocker : IResourceLocker
    {
        private readonly IRedLock redLock;

        public ResourceLocker(IRedLock redLock)
        {
            this.redLock = redLock ?? throw new ArgumentNullException(nameof(redLock));
        }

        public string Resource => redLock.Resource;

        public bool IsAcquired => redLock.IsAcquired;

        public void Dispose()
        {
            redLock.Dispose();
        }
    }
}
