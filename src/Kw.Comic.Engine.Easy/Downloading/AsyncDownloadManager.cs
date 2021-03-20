using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Kw.Comic.Engine.Easy.Concurrnets;

namespace Kw.Comic.Engine.Easy.Downloading
{
    public abstract class AsyncDownloadManager : ThreadSafeList<DownloadTask>, IDownloadManager
    {
        protected AsyncDownloadManager()
        {
        }


        private int isStart;
        private Task task;
        private CancellationTokenSource tokenSource;

        public bool IsStart => isStart != 1;

        public Task Task => task;

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
                var ok = GetDownloadTask();
                if (ok!=null)
                {
                    bool? res = true;
                    while (res==true&&!tk.IsCancellationRequested)
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
                await Task.Yield();
            }
        }
        protected virtual void OnException(DownloadTask task,Exception exception)
        {

        }
        protected virtual void OnComplated(DownloadTask task)
        {

        }
        protected abstract DownloadTask GetDownloadTask();
    }
}
