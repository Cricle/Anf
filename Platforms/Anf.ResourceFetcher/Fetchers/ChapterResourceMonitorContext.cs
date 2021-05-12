using Anf.ChannelModel.Mongo;

namespace Anf.ResourceFetcher.Fetchers
{
    internal class ChapterResourceMonitorContext : ResourceMonitorContext, IValueResourceMonitorContext<WithPageChapter>
    {
        public WithPageChapter Value { get; set; }
    }
}
