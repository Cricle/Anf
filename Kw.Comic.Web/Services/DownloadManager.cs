using Kw.Comic.Engine.Easy;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KwC.Services
{
    public class DownloadManager : IDisposable, IDownloadManager
    {
        private readonly IServiceProvider host;
        private readonly SemaphoreSlim semaphoreSlim;
        private readonly ConcurrentDictionary<string, DownloadTask> tasks;
        private readonly ITaskDispatch taskDispatch;
        private readonly IDownloadListener downloadListener;
        private readonly IServiceScope scope;
        private int total;
        private int current;

        public DownloadManager(IServiceProvider host)
        {
            this.host = host;
            scope = host.CreateScope();
            semaphoreSlim = new SemaphoreSlim(1,1);
            downloadListener = new NotifyListener(scope.ServiceProvider);
            tasks = new ConcurrentDictionary<string, DownloadTask>();
            taskDispatch = new TaskDispatch(this);
        }

        public ITaskDispatch TaskDispatch => taskDispatch;

        public TaskPosition Position => new TaskPosition(total,current);

        public event Action<DownloadManager, DownloadTask> Added;
        public event Action<DownloadManager, DownloadTask> Done;

        public event Action<DownloadManager, TaskPosition> PositionChanged;

        protected void RaisePositionChanged()
        {
            PositionChanged?.Invoke(this, Position);
        }

        public async Task<DownloadTask> AddAsync(string address)
        {
            if (tasks.TryGetValue(address,out var task))
            {
                return task;
            }
            await semaphoreSlim.WaitAsync();
            try
            {
                if (!tasks.TryGetValue(address, out task))
                {
                    var s = await host.CreateDownloadAsync(address);
                    if (s.Downloader == null)
                    {
                        return null;
                    }
                    s.Request.Listener = downloadListener;
                    var dt = new DownloadTask(address, s);
                    total += dt.Max;
                    RaisePositionChanged();
                    dt.MovedNext += OnMovedNext;
                    tasks.TryAdd(address, dt);
                    Added?.Invoke(this, dt);
                    return dt;
                }
                return task;
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        private void OnMovedNext(DownloadTask arg1, int arg2)
        {
            if (arg1.IsDone)
            {
                semaphoreSlim.Wait();
                try
                {
                    arg1.MovedNext -= OnMovedNext;
                    tasks.TryRemove(arg1.Address,out _);
                    Done?.Invoke(this, arg1);
                }
                finally
                {
                    semaphoreSlim.Release();
                }
            }
            semaphoreSlim.Wait();
            try
            {
                current++;
                RaisePositionChanged();
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public DownloadTask GetFirst()
        {
            if (tasks.Count==0)
            {
                return null;
            }
            return tasks.FirstOrDefault().Value;
        }
        public DownloadTask Get(string address)
        {
            if (tasks.TryGetValue(address, out var tsk))
            {
                return tsk;
            }
            return null;
        }

        public void Dispose()
        {
            taskDispatch.Dispose();
            semaphoreSlim.Wait();
            try
            {
                foreach (var item in tasks)
                {
                    item.Value.TokenSource.Cancel();
                }
                tasks.Clear();
            }
            finally
            {
                semaphoreSlim.Release();
            }
            semaphoreSlim.Dispose();
        }
    }
}
