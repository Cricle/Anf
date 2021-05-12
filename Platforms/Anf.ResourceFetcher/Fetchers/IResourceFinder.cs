using Anf.ChannelModel.Mongo;
using System.Threading.Tasks;

namespace Anf.ResourceFetcher.Fetchers
{
    public interface IResourceFinder
    {
        Task<AnfComicEntityTruck> FetchEntityAsync(IResourceFetchContext context);
        Task<WithPageChapter> FetchChapterAsync(IResourceFetchContext context);
    }
}
