using Anf.ChannelModel.Mongo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Anf.ResourceFetcher.Fetchers
{
    public class ResourceFetchGroup : List<IResourceFetcher>,IResourceFetcher
    {
        public async Task DoneFetchChapterAsync(IValueResourceMonitorContext<WithPageChapter> context)
        {
            foreach (var item in this)
            {
                await item.DoneFetchChapterAsync(context);
            }
        }

        public async Task DoneFetchEntityAsync(IValueResourceMonitorContext<AnfComicEntityTruck> context)
        {
            foreach (var item in this)
            {
                await item.DoneFetchEntityAsync(context);
            }
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
