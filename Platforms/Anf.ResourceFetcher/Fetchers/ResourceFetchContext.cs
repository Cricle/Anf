using Anf.ChannelModel.KeyGenerator;
using RedLockNet;
using System.Threading.Tasks;

namespace Anf.ResourceFetcher.Fetchers
{
    internal class ResourceFetchContext : IResourceFetchContext
    {
        private const string FetckKey = "Red.Anf.ResourceFetcher.Fetchers.ResourceFetchContext.FetchKey";

        private const string EntityKey = "Entity";
        private const string ChapterKey = "Chapter";

        private readonly IDistributedLockFactory distributedLockFactory;

        private bool requireReloop;

        public ResourceFetchContext(IDistributedLockFactory distributedLockFactory, 
            string url,
            IResourceFetcher requireReloopFetcher,
            IResourceFinder root)
        {
            this.distributedLockFactory = distributedLockFactory;
            Root = root;
            Url = url;
            RequireReloopFetcher = requireReloopFetcher;
        }

        public string Url { get; }

        public IResourceFetcher RequireReloopFetcher { get; }

        public bool RequireReloop => requireReloop;

        public IResourceFinder Root { get; }

        public Task<IRedLock> CreateEntityLockerAsync() => CreateLockerAsync(EntityKey);
        public Task<IRedLock> CreateChapterLockerAsync() => CreateLockerAsync(ChapterKey);

        private Task<IRedLock> CreateLockerAsync(string part)
        {
            var key = RedisKeyGenerator.Concat(FetckKey, part, Url);
            return distributedLockFactory.CreateLockAsync(key,RedisKeyGenerator.RedKeyOutTime);
        }
        public void SetRequireReloop()
        {
            requireReloop = true;
        }

        public IResourceFetchContext Copy(string url)
        {
            return new ResourceFetchContext(distributedLockFactory, url, RequireReloopFetcher, Root);
        }
    }
}
