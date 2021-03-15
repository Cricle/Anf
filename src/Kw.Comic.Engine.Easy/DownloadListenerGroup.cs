using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy
{
    public class DownloadListenerGroup : List<IDownloadListener>, IDownloadListener
    {
        public async Task BeginFetchPageAsync(DownloadListenerContext context)
        {
            foreach (var item in this)
            {
                await item.BeginFetchPageAsync(context);
            }
        }

        public async Task CanceledAsync(DownloadListenerContext context)
        {
            foreach (var item in this)
            {
                await item.CanceledAsync(context);
            }
        }

        public async Task ComplatedSaveAsync(DownloadListenerContext context)
        {
            foreach (var item in this)
            {
                await item.ComplatedSaveAsync(context);
            }
        }

        public async Task EndFetchPageAsync(DownloadListenerContext context)
        {
            foreach (var item in this)
            {
                await item.EndFetchPageAsync(context);
            }
        }

        public async Task FetchPageExceptionAsync(DownloadExceptionListenerContext context)
        {
            foreach (var item in this)
            {
                await item.FetchPageExceptionAsync(context);
            }
        }

        public async Task NotNeedToSaveAsync(DownloadListenerContext context)
        {
            foreach (var item in this)
            {
                await item.NotNeedToSaveAsync(context);
            }
        }

        public async Task ReadyFetchAsync(DownloadListenerContext context)
        {
            foreach (var item in this)
            {
                await item.ReadyFetchAsync(context);
            }
        }

        public async Task ReadySaveAsync(DownloadSaveListenerContext context)
        {
            foreach (var item in this)
            {
                await item.ReadySaveAsync(context);
            }
        }
    }
}
