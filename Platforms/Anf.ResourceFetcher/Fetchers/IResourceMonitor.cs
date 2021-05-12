using Anf.ChannelModel.Mongo;
using System.Threading.Tasks;

namespace Anf.ResourceFetcher.Fetchers
{
    public interface IResourceMonitor
    {
        Task DoneFetchEntityAsync(IValueResourceMonitorContext<AnfComicEntityTruck> context);
        Task DoneFetchChapterAsync(IValueResourceMonitorContext<WithPageChapter> context);
    }
}
