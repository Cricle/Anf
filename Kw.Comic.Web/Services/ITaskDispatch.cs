using System;

namespace KwC.Services
{
    public interface ITaskDispatch:IDisposable
    {
        DownloadTask Current { get; }

        event Action<DownloadTask> CurrentTaskChanged;
        event Action<DownloadTask,int> MovedNext;
    }
}
