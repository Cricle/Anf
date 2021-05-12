using RedLockNet;
using System.Threading.Tasks;

namespace Anf.ResourceFetcher.Fetchers
{
    public interface IResourceFetchContext
    {
        string Url { get; }

        IResourceFetcher RequireReloopFetcher { get; }

        IResourceFinder Root { get; }

        bool RequireReloop { get; }

        void SetRequireReloop();

        Task<IRedLock> CreateEntityLockerAsync();

        Task<IRedLock> CreateChapterLockerAsync();

        IResourceFetchContext Copy(string url);
    }
}
