using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KwC.Services
{
    public class TaskDispatch : ITaskDispatch,IDisposable
    {
        public TaskDispatch(IDownloadManager downloadManager)
        {
            Source = new CancellationTokenSource();
            Task = new Task(Run, downloadManager, TaskCreationOptions.LongRunning);
            Task.Start();
        }
        private DownloadTask current;
        public Task Task { get; }
        public CancellationTokenSource Source { get; }
        public DownloadTask Current => current;

        public event Action<DownloadTask, Exception> Exception;
        public event Action<DownloadTask> CurrentTaskChanged;
        public event Action<DownloadTask> Done;
        public event Action<DownloadTask, int> MovedNext;

        private async void Run(object mgr)
        {
            var downloadManager = (IDownloadManager)mgr;
            var source = Source;
            while (!source.IsCancellationRequested)
            {
                try
                {
                    current = downloadManager.GetFirst();
                    if (current != null)
                    {
                        CurrentTaskChanged?.Invoke(current);
                        current.MovedNext += OnMovedNext;
                        while (await current.MoveNextAsync())
                        {
                            await Task.Yield();
                        }
                        Done?.Invoke(current);
                        current = null;
                    }
                }
                catch (Exception ex)
                {
                    Exception?.Invoke(current, ex);
                }
                finally
                {
                    if (current != null)
                    {
                        current.MovedNext -= OnMovedNext;
                    }
                }
                await Task.Delay(500);
            }
        }

        private void OnMovedNext(DownloadTask arg1, int arg2)
        {
            MovedNext?.Invoke(arg1, arg2);
        }

        public void Dispose()
        {
            Source.Cancel();
            Source.Dispose();
        }
    }
}
