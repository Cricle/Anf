using Microsoft.IO;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy.Visiting
{
    internal class SharedComic : ISharedComic, IDisposable
    {
        private readonly ConcurrentDictionary<string, Task<Stream>> streams;
        private readonly IComicSourceProvider sourceProvider;
        private readonly RecyclableMemoryStreamManager recyclableMemoryStreamManager;

        public SharedComic(IComicSourceProvider sourceProvider, RecyclableMemoryStreamManager recyclableMemoryStreamManager, int? capacity)
        {
            this.streams = new ConcurrentDictionary<string, Task<Stream>>();
            this.sourceProvider = sourceProvider;
            this.recyclableMemoryStreamManager = recyclableMemoryStreamManager;
            Capacity = capacity;
        }

        public int? Capacity { get; }

        public void Add(string key,Task<Stream> value)
        {
            streams.AddOrUpdate(key, value, (_, old) =>
            {
                CoreRemove(old);
                return value;
            });
        }

        public Stream Get(string address)
        {
            if (streams.TryGetValue(address, out var s)&&s.IsCompleted)
            {
                return s.Result;
            }
            return null;
        }

        public Task<Stream> GetAsync(string address)
        {
            if (streams.TryGetValue(address,out var s))
            {
                return s;
            }
            return Task.FromResult<Stream>(null);
        }
        private void CoreRemove(Task<Stream> s)
        {
            if (s.IsCompleted)
            {
                s.Result?.Dispose();
            }
            else
            {
                s.ContinueWith(a => a.Result?.Dispose());
            }

        }
        private void ThrowOneIfNeed()
        {
            if (Capacity != null && Capacity != 0 && streams.Count > Capacity)
            {
                streams.TryRemove(streams.Keys.First(), out var s);
                CoreRemove(s);
            }
        }
        public Task<Stream> GetOrLoadAsync(string address)
        {
            ThrowOneIfNeed();
            return streams.GetOrAdd(address, addr => DownloadAsync(addr));
        }
        protected virtual async Task<Stream> DownloadAsync(string address)
        {
            var mem = recyclableMemoryStreamManager.GetStream();
            using (var remoteStream = await sourceProvider.GetImageStreamAsync(address))
            {
                await remoteStream.CopyToAsync(mem);
            }
            return mem;
        }

        public void Dispose()
        {
            Clear();
        }
        public void Clear()
        {
            foreach (var item in streams)
            {
                CoreRemove(item.Value);
            }
        }
    }
}
