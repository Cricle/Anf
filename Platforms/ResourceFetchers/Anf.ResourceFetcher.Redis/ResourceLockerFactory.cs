using Anf.ChannelModel.KeyGenerator;
using Microsoft.Extensions.Options;
using RedLockNet;
using System;
using System.Threading.Tasks;

namespace Anf.ResourceFetcher.Redis
{
    internal class ResourceLockerFactory : IResourceLockerFactory
    {
        private readonly IDistributedLockFactory distributedLockFactory;
        private readonly IOptions<ResourceLockOptions> resourceLockOptions;

        public ResourceLockerFactory(IDistributedLockFactory distributedLockFactory, IOptions<ResourceLockOptions> resourceLockOptions)
        {
            this.distributedLockFactory = distributedLockFactory;
            this.resourceLockOptions = resourceLockOptions;
        }

        private TimeSpan LockTime => resourceLockOptions?.Value?.ResourceLockTimeout ?? RedisKeyGenerator.RedKeyOutTime;
        public IResourceLocker CreateLocker(string resource)
        {
            var locker= distributedLockFactory.CreateLock(resource, LockTime);
            return new ResourceLocker(locker);
        }

        public async Task<IResourceLocker> CreateLockerAsync(string resource)
        {
            var locker =await distributedLockFactory.CreateLockAsync(resource, LockTime);
            return new ResourceLocker(locker);
        }
    }
}
