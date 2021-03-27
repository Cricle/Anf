using Kw.Comic.Engine;
using Kw.Comic.Engine.Easy.Store;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Kw.Comic.Web.Services
{
    public class EnginePicker : IDisposable
    {
        private readonly IStoreService storeService;
        private readonly IServiceScope scope;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ComicEngine comicEngine;

        public EnginePicker(ComicEngine comicEngine,
            IServiceScopeFactory serviceScopeFactory,
            IStoreService storeService)
        {
            this.storeService = storeService;
            this.comicEngine=comicEngine;
            this.serviceScopeFactory = serviceScopeFactory;
            scope = serviceScopeFactory.CreateScope();
        }
        public string GetProviderIdentity(string address)
        {
            var type = comicEngine.GetComicSourceProviderType(address);
            return type?.EnginName;
        }
        public async Task<string> GetImageStreamAsync(string provider, string address)
        {
            var url=await storeService.GetPathAsync(address);
            if (File.Exists(url))
            {
                return url;
            }
            var type = comicEngine.FirstOrDefault(x => x.EnginName== provider);
            if (type == null)
            {
                return null;
            }
            var ser = (IComicSourceProvider)scope.ServiceProvider.GetRequiredService(type.ProviderType);
            using var stream=await ser.GetImageStreamAsync(address);
            url=await storeService.SaveAsync(address, stream);
            return url;
        }

        public void Dispose()
        {
            scope.Dispose();
        }
    }
}
