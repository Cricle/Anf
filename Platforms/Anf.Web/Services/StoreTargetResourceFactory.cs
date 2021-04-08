using Anf;
using Anf.Easy.Store;
using Anf.Easy.Visiting;
using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Anf.Web.Services
{
    public class StoreTargetResourceFactory : IResourceFactory<string>
    {
        private readonly IStoreService storeService;
        private readonly IComicSourceProvider comicSourceProvider;
        private readonly SemaphoreSlim semaphoreSlim;

        public StoreTargetResourceFactory(IStoreService storeService,
            IComicSourceProvider comicSourceProvider)
        {
            this.storeService = storeService;
            this.comicSourceProvider = comicSourceProvider;
            semaphoreSlim = new SemaphoreSlim(1);
        }

        public void Dispose()
        {
            semaphoreSlim.Wait();
            semaphoreSlim.Dispose();
        }
        public async Task<string> GetAsync(string address)
        {
            var path = await storeService.GetPathAsync(address);
            if (File.Exists(path))
            {
                return path;
            }
            await semaphoreSlim.WaitAsync();
            try
            {
                using var remoteStream = await comicSourceProvider.GetImageStreamAsync(address);
                path = await storeService.SaveAsync(address, remoteStream);
                return path;
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
    }
}
