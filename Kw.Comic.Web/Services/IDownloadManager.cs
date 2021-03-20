using System;
using System.Threading.Tasks;

namespace KwC.Services
{
    public interface IDownloadManager
    {
        ITaskDispatch TaskDispatch { get; }

        event Action<DownloadManager, DownloadTask> Added;
        event Action<DownloadManager, DownloadTask> Done;

        Task<DownloadTask> AddAsync(string address);
        void Dispose();
        DownloadTask Get(string address);
        DownloadTask GetFirst();
    }
}