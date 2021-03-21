using Kw.Comic.Engine.Easy.Store;
using Kw.Comic.Engine.Easy.Visiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IO;
using System.Threading.Tasks;

namespace KwC.Services
{
    public class StoreResourceCreatorFactory : IResourceFactoryCreator<string>
    {
        public Task<IResourceFactory<string>> CreateAsync(ResourceFactoryCreateContext<string> context)
        {
            var storeServices = context.Visiting.Host.GetRequiredService<IStoreService>();
            var streamManager = context.Visiting.Host.GetRequiredService<RecyclableMemoryStreamManager>();
            return Task.FromResult<IResourceFactory<string>>(
                new StoreTargetResourceFactory(storeServices,
                context.SourceProvider,
                streamManager));
        }
    }
}
