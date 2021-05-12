using Anf.ChannelModel.Mongo;

namespace Anf.ResourceFetcher.Fetchers
{
    internal class EntityResourceMonitorContext : ResourceMonitorContext, IValueResourceMonitorContext<AnfComicEntityTruck>
    {
        public AnfComicEntityTruck Value { get; set; }
    }
}
