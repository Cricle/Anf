using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy
{
    public abstract class DownloadListenerBase : IDownloadListener
    {
        public virtual Task BeginFetchPageAsync(DownloadListenerContext context)
        {
#if NETSTANDARD2_0
            return Task.CompletedTask;
#else
            return Task.FromResult(false);
#endif
        }

        public virtual Task CanceledAsync(DownloadListenerContext context)
        {
#if NETSTANDARD2_0
            return Task.CompletedTask;
#else
            return Task.FromResult(false);
#endif
        }

        public virtual Task ComplatedSaveAsync(DownloadListenerContext context)
        {
#if NETSTANDARD2_0
            return Task.CompletedTask;
#else
            return Task.FromResult(false);
#endif
        }

        public virtual Task EndFetchPageAsync(DownloadListenerContext context)
        {
#if NETSTANDARD2_0
            return Task.CompletedTask;
#else
            return Task.FromResult(false);
#endif
        }

        public virtual Task FetchPageExceptionAsync(DownloadExceptionListenerContext context)
        {
#if NETSTANDARD2_0
            return Task.CompletedTask;
#else
            return Task.FromResult(false);
#endif
        }

        public virtual Task NotNeedToSaveAsync(DownloadListenerContext context)
        {
#if NETSTANDARD2_0
            return Task.CompletedTask;
#else
            return Task.FromResult(false);
#endif
        }

        public virtual Task ReadyFetchAsync(DownloadListenerContext context)
        {
#if NETSTANDARD2_0
            return Task.CompletedTask;
#else
            return Task.FromResult(false);
#endif
        }

        public virtual Task ReadySaveAsync(DownloadSaveListenerContext context)
        {
#if NETSTANDARD2_0
            return Task.CompletedTask;
#else
            return Task.FromResult(false);
#endif
        }
    }
}
