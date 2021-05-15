using Anf.ChannelModel.Mongo;
using System.Threading.Tasks;

namespace Anf.ResourceFetcher.Fetchers
{
    public interface ISingleResourceFinder
    {
        Task<AnfComicEntityTruck> FetchEntityAsync(IResourceFetchContext context);
        Task<WithPageChapter> FetchChapterAsync(IResourceFetchContext context);
    }
}
