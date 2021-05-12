using Anf.Easy.Store;
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

            public string Address { get; }
            public StoreComicVisiting<Stream> Visiting { get; }

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
        private readonly LruCacher<string, VisitingBox> visitings;

        public SharedComicVisiting(int max)
        {
            visitings = new LruCacher<string, VisitingBox>(max);
            visitings.Removed += OnVisitingsRemoved;
        }

        public IReadOnlyCollection<string> IncludeAddress => visitings.Datas.Keys.ToArray();

        public int Count => visitings.Count;

        public bool UseCDN { get; set; } = false;

        public bool UseStore { get; set; } = false;

        public async Task<StoreComicVisiting<Stream>> GetAsync(string address)
        {
            var box = visitings.GetOrAdd(address, () => new VisitingBox(address, UseStore, UseCDN));
            await box.InitAsync();
            return box.Visiting;
        }
        private void OnVisitingsRemoved(string arg1, VisitingBox arg2)
        {
            arg2.Dispose();
        }
    }
}
