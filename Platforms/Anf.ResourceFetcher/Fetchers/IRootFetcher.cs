using Anf.ChannelModel.Mongo;
using System.Threading.Tasks;

namespace Anf.ResourceFetcher.Fetchers
{
    public interface IRootSingleFetcher : ISingleResourceFetcher
    {
        Task<WithPageChapter> FetchChapterAsync(string url, string entityUrl);
        Task<AnfComicEntityTruck> FetchEntityAsync(string url);
    }
    public interface IRootBatchFetcher : IBatchResourceFetcher
    {
        Task<WithPageChapter[]> FetchChaptersAsync(FetchChapterIdentity[] identities);
        Task<AnfComicEntityTruck[]> FetchEntitysAsync(FetchChapterIdentity[] identities);
    }
    public interface IRootFetcher : IRootSingleFetcher,IBatchResourceFetcher
    {

    }
}
