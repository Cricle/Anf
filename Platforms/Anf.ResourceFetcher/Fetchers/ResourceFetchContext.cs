using Ao.Cache;
using SecurityLogin;
using System;
using System.Threading.Tasks;

namespace Anf.ResourceFetcher.Fetchers
{
    internal class ResourceFetchContext : IResourceFetchContext
    {
        private const string FetckKey = "Red.Anf.ResourceFetcher.Fetchers.ResourceFetchContext.FetchKey";

        private const string EntityKey = "Entity";
        private const string ChapterKey = "Chapter";

        private readonly IResourceLockerFactory resourceLockerFactory;

        private bool isFromCache;
        private bool requireReloop;

        public ResourceFetchContext(IResourceLockerFactory resourceLockerFactory,
            string url,
            ISingleResourceFetcher requireReloopFetcher,
            ISingleResourceFinder root,
            string entityUrl)
        {
            EntityUrl = entityUrl;
            this.resourceLockerFactory = resourceLockerFactory;
            Root = root;
            Url = url;
            RequireReloopFetcher = requireReloopFetcher;
        }

        public string Url { get; }

        public ISingleResourceFetcher RequireReloopFetcher { get; }

        public bool IsFromCache => isFromCache;

        public bool RequireReloop => requireReloop;

        public ISingleResourceFinder Root { get; }

        public string EntityUrl { get; }

        public bool SupportLocker => resourceLockerFactory != null;

        public Task<IResourceLocker> CreateEntityLockerAsync() => CreateLockerAsync(EntityKey);
        public Task<IResourceLocker> CreateChapterLockerAsync() => CreateLockerAsync(ChapterKey);

        private Task<IResourceLocker> CreateLockerAsync(string part)
        {
            if (resourceLockerFactory is null)
            {
                throw new NotSupportedException("The IResourceLockerFactory is not provided, the function is not support");
            }
            var key = KeyGenerator.Concat(FetckKey, part, Url);
            return resourceLockerFactory.CreateLockerAsync(key);
        }
        public void SetRequireReloop()
        {
            requireReloop = true;
        }

        public IResourceFetchContext Copy(string url)
        {
            return new ResourceFetchContext(resourceLockerFactory, url, RequireReloopFetcher, Root, EntityUrl);
        }

        public void SetIsCache()
        {
            isFromCache = true;
        }
    }
}
