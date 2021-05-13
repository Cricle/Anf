using Anf.ChannelModel.Mongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.ResourceFetcher.Fetchers
{
    public class ResourceFetchGroup : List<IResourceFetcher>,IResourceFetcher
    {
        public Task DoneFetchChapterAsync(IValueResourceMonitorContext<WithPageChapter> context)
        {
            return Task.WhenAll(this.Select(x => x.DoneFetchChapterAsync(context)).ToArray());
        }

        public Task DoneFetchEntityAsync(IValueResourceMonitorContext<AnfComicEntityTruck> context)
        {
            return Task.WhenAll(this.Select(x => x.DoneFetchEntityAsync(context)).ToArray());
        }

        public async Task<WithPageChapter> FetchChapterAsync(IResourceFetchContext context)
        {
            for (int i = 0; i < Count; i++)
            {
                var c = this[i];
                var res = await c.FetchChapterAsync(context);
                if (res !=null)
                {
                    return res;
                }
            }
            return null;
        }

        public async Task<AnfComicEntityTruck> FetchEntityAsync(IResourceFetchContext context)
        {
            for (int i = 0; i < Count; i++)
            {
                var c = this[i];
                var res = await c.FetchEntityAsync(context);
                if (res != null)
                {
                    return res;
                }
            }
            return null;
        }
    }
}
