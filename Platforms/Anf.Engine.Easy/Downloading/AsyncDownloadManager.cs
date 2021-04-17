using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Anf.Easy.Concurrnets;

namespace Anf.Easy.Downloading
{
    public abstract class AsyncDownloadManager : KwSynchronizedCollection<DownloadTask>, IDownloadManager
    {

        public static readonly TimeSpan DefaultWaitTime = TimeSpan.FromMilliseconds(200);

        protected AsyncDownloadManager()
        {
        }


        private int isStart;
        private Task task;
        private CancellationTokenSource tokenSource;

        public bool IsStart => isStart != 0;

        public Task Task => task;

        public TimeSpan WaitTime { get; set; } = DefaultWaitTime;

        public CancellationToken CancellationToken => tokenSource.Token;

        public void Start()
        {
            if (Interlocked.CompareExchange(ref isStart, 1, 0) == 0)
            {
                tokenSource?.Dispose();
                tokenSource = new CancellationTokenSource();
                ReadyStart();
                task = Task.Factory.StartNew(LoopDownload, tokenSource.Token, tokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
                OnStart();
            }
        }
        protected virtual void ReadyStart()
        {

        }
        protected virtual void OnStart()
        {

        }

        public void Stop()
        {
            if (Interlocked.CompareExchange(ref isStart, 0, 1) == 1)
            {
                tokenSource.Cancel();
                OnStop();
            }
        }
        protected virtual void OnStop()
        {

        }

        private async void LoopDownload(object token)
        {
            var tk = (CancellationToken)token;
            while (!tk.IsCancellationRequested)
            {
                var begin = Stopwatch.GetTimestamp();
                var ok = GetDownloadTask();
                if (ok != null)
                {
                    bool? res = true;
                    while (res == true && !tk.IsCancellationRequested)
                    {
                        try
                        {
                            res = await ok.MoveNextAsync();
                        }
                        catch (Exception ex)
                        {
                            OnException(ok, ex);
                        }
                    }
                    OnComplated(ok);
                    Remove(ok);
                }
                if (Count == 0)
                {
                    var end = Stopwatch.GetTimestamp();
                    var waitTime = (int)(end - begin - (long)WaitTime.TotalMilliseconds);
                    if (waitTime > 0)
                    {
                        await Task.Delay(waitTime);
                    }
                }
            }
        }
        protected virtual void OnException(DownloadTask task, Exception exception)
        {

        }
        protected virtual void OnComplated(DownloadTask task)
        {

        }
        protected abstract DownloadTask GetDownloadTask();
    }
}
