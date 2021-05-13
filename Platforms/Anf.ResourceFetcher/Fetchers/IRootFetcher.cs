using Anf.ChannelModel.Mongo;
using System.Threading.Tasks;

namespace Anf.ResourceFetcher.Fetchers
{
    public interface IRootFetcher : IResourceFetcher
    {
        Task<WithPageChapter> FetchChapterAsync(string url,string entityUrl);
        Task<AnfComicEntityTruck> FetchEntityAsync(string url);
    }
}
