using RedLockNet;
using System.Threading.Tasks;

namespace Anf.ResourceFetcher.Fetchers
{
    public interface IResourceFetchContext
    {
        string Url { get; }

        IResourceFetcher RequireReloopFetcher { get; }

        IResourceFinder Root { get; }

        string EntityUrl { get; }

        bool RequireReloop { get; }

        bool IsFromCache { get; }

        void SetRequireReloop();

        void SetIsCache();

        Task<IRedLock> CreateEntityLockerAsync();

        Task<IRedLock> CreateChapterLockerAsync();

        IResourceFetchContext Copy(string url);
    }
}
