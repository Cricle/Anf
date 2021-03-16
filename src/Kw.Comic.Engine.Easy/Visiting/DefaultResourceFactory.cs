using System;
using System.IO;
using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy.Visiting
{
    public class DefaultResourceFactory : IResourceFactoryCreator
    {
        public static readonly DefaultResourceFactory Default = new DefaultResourceFactory();

        public Task<IResourceFactory> CreateAsync(ResourceFactoryCreateContext context)
        {
            return Task.FromResult<IResourceFactory>(new ResourceFactory(context.SourceProvider));
        }

        public void Dispose()
        {
        }
        class ResourceFactory : IResourceFactory
        {
            private readonly IComicSourceProvider sourceProvider;

            public ResourceFactory(IComicSourceProvider sourceProvider)
            {
                this.sourceProvider = sourceProvider ?? throw new ArgumentNullException(nameof(sourceProvider));
            }

            public void Dispose()
            {
            }

            public Task<Stream> GetAsync(string address)
            {
                return sourceProvider.GetImageStreamAsync(address);
            }
        }
    }
}
