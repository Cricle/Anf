using Anf.Easy.Visiting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Anf.Web
{
    internal class WebResourceFactory : IResourceFactory<Stream>
    {
        private readonly ResourceFactoryCreateContext<Stream> context;

        public WebResourceFactory(ResourceFactoryCreateContext<Stream> context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Dispose()
        {

        }

        public Task<Stream> GetAsync(string address)
        {
            var eng = context.ServiceProvider.GetRequiredService<ComicEngine>();
            var type = eng.GetComicSourceProviderType(address);
            if ( type is null)
            {
                return Task.FromResult<Stream>(null);
            }
            var provider = (IComicSourceProvider)context.ServiceProvider.GetRequiredService(type.ProviderType);
            return provider.GetImageStreamAsync(address);
        }
    }
}
