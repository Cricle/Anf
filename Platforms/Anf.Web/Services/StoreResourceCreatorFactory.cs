using Anf.Easy.Store;
using Anf.Easy.Visiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IO;
using System.Threading.Tasks;

namespace Anf.Web.Services
{
    public class StoreResourceCreatorFactory : IResourceFactoryCreator<string>
    {
        public Task<IResourceFactory<string>> CreateAsync(ResourceFactoryCreateContext<string> context)
        {
            var storeServices = context.Visiting.Host.GetRequiredService<IStoreService>();
            return Task.FromResult<IResourceFactory<string>>(
                new StoreTargetResourceFactory(storeServices,
                context.SourceProvider));
        }
    }
}
