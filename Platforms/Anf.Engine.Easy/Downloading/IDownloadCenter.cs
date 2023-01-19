using System;
using System.Threading.Tasks;

namespace Anf.Easy.Downloading
{
    public interface IDownloadCenter
    {
        IComicSaver Saver { get; }

        event Action<DownloadCenter, DownloadBox> Added;
        event Action<DownloadCenter> Cleared;
        event Action<DownloadCenter, DownloadBox> Done;
        event Action<DownloadCenter, DownloadTask, int> MovedNext;
        event Action<DownloadCenter, DownloadBox> Removed;
        event Action<DownloadCenter, DownloadTask, int> Seeked;

        Task AddAsync(DownloadLink link);
        Task AddAsync(string address, IDownloadListener downloadListener = null);
        Task ClearAsync(bool cancel = true);
        Task<bool> RemoveAsync(string address, bool cancel = true);
    }
}