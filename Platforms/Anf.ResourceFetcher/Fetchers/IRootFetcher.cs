using Anf.ChannelModel.Mongo;
using System.Threading.Tasks;

namespace Anf.ResourceFetcher.Fetchers
{
    public interface IRootFetcher : IResourceFetcher
    {
        Task<WithPageChapter> FetchChapterAsync(string url);
        Task<AnfComicEntityTruck> FetchEntityAsync(string url);
    }
}
