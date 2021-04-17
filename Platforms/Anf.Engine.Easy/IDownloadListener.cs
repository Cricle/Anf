using System.Threading.Tasks;

namespace Anf.Easy
{
    public interface IDownloadListener
    {
        Task ReadyFetchAsync(DownloadListenerContext context);

        Task NotNeedToSaveAsync(DownloadListenerContext context);

        Task CanceledAsync(DownloadListenerContext context);

        Task BeginFetchPageAsync(DownloadListenerContext context);

        Task FetchPageExceptionAsync(DownloadExceptionListenerContext context);

        Task EndFetchPageAsync(DownloadListenerContext context);

        Task ReadySaveAsync(DownloadSaveListenerContext context);

        Task ComplatedSaveAsync(DownloadListenerContext context);
    }
}
