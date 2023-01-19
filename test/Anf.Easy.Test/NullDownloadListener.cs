using System.Threading.Tasks;

namespace Anf.Easy.Test
{
    internal class NullDownloadListener : IDownloadListener
    {
        public bool IsBeginFetchPageAsync { get; set; }
        public bool IsCanceledAsync { get; set; }
        public bool IsComplatedSaveAsync { get; set; }
        public bool IsEndFetchPageAsync { get; set; }
        public bool IsFetchPageExceptionAsync { get; set; }
        public bool IsNotNeedToSaveAsync { get; set; }
        public bool IsReadyFetchAsync { get; set; }
        public bool IsReadySaveAsync { get; set; }

        public Task BeginFetchPageAsync(DownloadListenerContext context)
        {
            IsBeginFetchPageAsync = true;
            return Task.FromResult(0);
        }

        public Task CanceledAsync(DownloadListenerContext context)
        {
            IsCanceledAsync = true;
            return Task.FromResult(0);
        }

        public Task ComplatedSaveAsync(DownloadListenerContext context)
        {
            IsComplatedSaveAsync = true;
            return Task.FromResult(0);
        }

        public Task EndFetchPageAsync(DownloadListenerContext context)
        {
            IsEndFetchPageAsync = true;
            return Task.FromResult(0);
        }

        public Task FetchPageExceptionAsync(DownloadExceptionListenerContext context)
        {
            IsFetchPageExceptionAsync = true;
            return Task.FromResult(0);
        }

        public Task NotNeedToSaveAsync(DownloadListenerContext context)
        {
            IsNotNeedToSaveAsync = true;
            return Task.FromResult(0);
        }

        public Task ReadyFetchAsync(DownloadListenerContext context)
        {
            IsReadyFetchAsync = true;
            return Task.FromResult(0);
        }

        public Task ReadySaveAsync(DownloadSaveListenerContext context)
        {
            IsReadySaveAsync = true;
            return Task.FromResult(0);
        }
    }
}
