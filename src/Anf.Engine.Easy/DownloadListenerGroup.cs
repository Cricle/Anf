using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anf.Easy
{
    public class DownloadListenerGroup : List<IDownloadListener>, IDownloadListener
    {
        public Task BeginFetchPageAsync(DownloadListenerContext context)
        {
            return Task.WhenAll(this.Where(x=>x!=null).Select(x => x.BeginFetchPageAsync(context)));
        }

        public Task CanceledAsync(DownloadListenerContext context)
        {
            return Task.WhenAll(this.Where(x => x != null).Select(x => x.CanceledAsync(context)));
        }

        public Task ComplatedSaveAsync(DownloadListenerContext context)
        {
            return Task.WhenAll(this.Where(x => x != null).Select(x => x.ComplatedSaveAsync(context)));
        }

        public Task EndFetchPageAsync(DownloadListenerContext context)
        {
            return Task.WhenAll(this.Where(x => x != null).Select(x => x.EndFetchPageAsync(context)));
        }

        public Task FetchPageExceptionAsync(DownloadExceptionListenerContext context)
        {
            return Task.WhenAll(this.Where(x => x != null).Select(x => x.FetchPageExceptionAsync(context)));
        }

        public Task NotNeedToSaveAsync(DownloadListenerContext context)
        {
            return Task.WhenAll(this.Where(x => x != null).Select(x => x.NotNeedToSaveAsync(context)));
        }

        public Task ReadyFetchAsync(DownloadListenerContext context)
        {
            return Task.WhenAll(this.Where(x => x != null).Select(x => x.ReadyFetchAsync(context)));
        }
        
        public Task ReadySaveAsync(DownloadSaveListenerContext context)
        {
            return Task.WhenAll(this.Where(x => x != null).Select(x => x.ReadySaveAsync(context)));
        }
    }
}
