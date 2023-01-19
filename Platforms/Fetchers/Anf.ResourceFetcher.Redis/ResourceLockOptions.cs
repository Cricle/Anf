using System;

namespace Anf.ResourceFetcher.Redis
{
    public class ResourceLockOptions
    {
        public TimeSpan ResourceLockTimeout { get; set; } = TimeSpan.FromSeconds(5);
    }
}
