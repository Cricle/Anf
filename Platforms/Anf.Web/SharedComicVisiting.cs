using Anf.Easy.Visiting;
using Anf.Platform;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Anf.Web
{
    public class SharedComicVisiting
    {
        internal class VisitingBox : IDisposable
        {
            private readonly IServiceScope scope;
            private readonly Task loadingTask;
            private int refCount;

            public VisitingBox(string address,bool useStore,bool useCDN)
            {
                Address = address;
                scope = AppEngine.CreateScope();
                var creator = scope.ServiceProvider.GetRequiredService<IResourceFactoryCreator<Stream>>();
                Visiting = new StoreComicVisiting<Stream>(scope.ServiceProvider, creator)
                {
                    UseStore = useStore,
                    EnableCDNCache = useCDN
                };
                loadingTask = Visiting.LoadAsync(Address);
            }

            public int RefCount => Volatile.Read(ref refCount);
            public string Address { get; }
            public StoreComicVisiting<Stream> Visiting { get; }

            public void AddRef()
            {
                Interlocked.Increment(ref refCount);
            }
            public void ReleaseRef()
            {
                Interlocked.Decrement(ref refCount);
            }

            public Task InitAsync()
            {
                if (loadingTask.IsCompleted)
                {
                    return Task.CompletedTask;
                }
                return loadingTask;
            }

            public void Dispose()
            {
                scope.Dispose();
                Visiting.Dispose();
            }
        }
        private readonly object locker;
        private readonly ConcurrentDictionary<string, VisitingBox> visitings;

        public SharedComicVisiting()
        {
            locker = new object();
            visitings = new ConcurrentDictionary<string, VisitingBox>();
        }

        public IReadOnlyCollection<string> IncludeAddress => visitings.Keys.ToArray();

        public int Count => visitings.Count;

        public bool UseCDN { get; set; } = true;

        public bool UseStore { get; set; }

        public async Task<StoreComicVisiting<Stream>> JoinAsync(string address)
        {
            VisitingBox box;
            lock (locker)
            {
                box = visitings.GetOrAdd(address, addr => new VisitingBox(addr,UseStore,UseCDN));
                box.AddRef();
            }
            await box.InitAsync();
            return box.Visiting;
        }
        public void Leave(string address)
        {
            lock (locker)
            {
                if (visitings.TryGetValue(address, out var box))
                {
                    box.ReleaseRef();
                    if (box.RefCount <= 0)
                    {
                        box.Dispose();
                        visitings.Remove(address, out _);
                    }
                }
            }
        }
    }
}
