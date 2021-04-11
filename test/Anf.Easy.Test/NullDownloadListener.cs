using System.Threading.Tasks;

namespace Anf.Easy.Test
{
    internal class NullDownloadListener : IDownloadListener
    {
        public Task BeginFetchPageAsync(DownloadListenerContext context)
        {
            return Task.FromResult(0);
        }

        public Task CanceledAsync(DownloadListenerContext context)
        {
            return Task.FromResult(0);
        }

        public Task ComplatedSaveAsync(DownloadListenerContext context)
        {
            return Task.FromResult(0);
        }

        public Task EndFetchPageAsync(DownloadListenerContext context)
        {
            return Task.FromResult(0);
        }

        public Task FetchPageExceptionAsync(DownloadExceptionListenerContext context)
        {
            return Task.FromResult(0);
        }

        public Task NotNeedToSaveAsync(DownloadListenerContext context)
        {
            return Task.FromResult(0);
        }

        public Task ReadyFetchAsync(DownloadListenerContext context)
        {
            return Task.FromResult(0);
        }

        public Task ReadySaveAsync(DownloadSaveListenerContext context)
        {
            return Task.FromResult(0);
        }
    }
}
