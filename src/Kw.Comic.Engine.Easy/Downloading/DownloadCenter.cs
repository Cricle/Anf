using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy.Downloading
{
    public class DownloadCenter : IOperatable, IReadOnlyDictionary<string, DownloadBox>, IDisposable, IDownloadCenter
    {
        private readonly Dictionary<string, DownloadBox> downloadMap;
        private readonly IServiceProvider serviceProvider;
        private readonly IDownloadManager downloadManager;
        private readonly SemaphoreSlim semaphoreSlim;

        public DownloadCenter(IServiceProvider serviceProvider, IDownloadManager downloadTasks, IComicSaver saver)
        {
            this.serviceProvider = serviceProvider;
            this.downloadManager = downloadTasks;
            Saver = saver;
            semaphoreSlim = new SemaphoreSlim(1, 1);
            downloadMap = new Dictionary<string, DownloadBox>();
        }

        public IComicSaver Saver { get; }

        public IEnumerable<string> Keys => downloadMap.Keys;

        public IEnumerable<DownloadBox> Values => downloadMap.Values;

        public int Count => downloadMap.Count;

        public bool IsStart => downloadManager.IsStart;

        public DownloadBox this[string key] => downloadMap[key];

        public event Action<DownloadCenter, DownloadBox> Added;
        public event Action<DownloadCenter, DownloadBox> Removed;
        public event Action<DownloadCenter, DownloadBox> Done;
        public event Action<DownloadCenter, DownloadTask, int> MovedNext;
        public event Action<DownloadCenter, DownloadTask, int> Seeked;
        public event Action<DownloadCenter> Cleared;

        private void AddCore(DownloadLink link)
        {
            var tokeSource = new CancellationTokenSource();
            var tasks = link.Downloader.EmitTasks(link.Request, tokeSource.Token);
            var downloadTask = new DownloadTask(tasks, tokeSource.Token);
            var box = new DownloadBox(downloadTask, link) { TokenSource = tokeSource };
            box.Canceled += Box_Canceled;
            downloadTask.MovedNext += (a, b) =>
            {
                MovedNext?.Invoke(this, a, b);
            };
            downloadTask.Seeked += (a, b) =>
            {
                Seeked?.Invoke(this, a, b);
            };
            downloadTask.Done += async w =>
            {
                await RemoveAsync(box.Address);
                Done?.Invoke(this, box);
            };
            downloadMap.Add(link.Request.Entity.ComicUrl, box);
            downloadManager.Add(downloadTask);
            Added?.Invoke(this, box);
        }

        private async void Box_Canceled(DownloadBox obj)
        {
            await RemoveAsync(obj.Address, false);
            obj.Canceled -= Box_Canceled;
        }

        public async Task AddAsync(DownloadLink link)
        {
            var url = link.Request.Entity.ComicUrl;
            if (downloadMap.ContainsKey(url))
            {
                return;
            }
            await semaphoreSlim.WaitAsync();
            try
            {
                if (downloadMap.ContainsKey(url))
                {
                    return;
                }
                AddCore(link);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
        public async Task ClearAsync(bool cancel = true)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                if (cancel)
                {
                    foreach (var item in Values)
                    {
                        item.Cancel();
                    }
                }
                downloadMap.Clear();
                Cleared?.Invoke(this);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
        public async Task<bool> RemoveAsync(string address, bool cancel = true)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                if (!downloadMap.TryGetValue(address, out var box))
                {
                    return false;
                }
                if (cancel)
                {
                    downloadMap[address].Cancel();
                }
                downloadMap.Remove(address);
                Removed?.Invoke(this, box);
                return true;
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public async Task AddAsync(string address, IDownloadListener downloadListener = null)
        {
            if (downloadMap.ContainsKey(address))
            {
                return;
            }
            await semaphoreSlim.WaitAsync();
            try
            {
                if (downloadMap.ContainsKey(address))
                {
                    return;
                }
                var link = await serviceProvider.MakeDownloadAsync(address, Saver);
                link.Request.Listener = downloadListener;
                AddCore(link);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public bool ContainsKey(string key)
        {
            return downloadMap.ContainsKey(key);
        }

        public bool TryGetValue(string key, out DownloadBox value)
        {
            return downloadMap.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<string, DownloadBox>> GetEnumerator()
        {
            return downloadMap.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Start()
        {
            downloadManager.Start();
        }

        public void Stop()
        {
            downloadManager.Stop();
        }

        public async void Dispose()
        {
            await semaphoreSlim.WaitAsync();
            Stop();
            await ClearAsync();
            semaphoreSlim.Dispose();
        }
    }
}
