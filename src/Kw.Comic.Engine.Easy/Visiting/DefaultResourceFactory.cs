using System;
using System.IO;
using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy.Visiting
{
    public class StreamResourceFactory : IResourceFactoryCreator<Stream>
    {
        public static readonly StreamResourceFactory Default = new StreamResourceFactory();

        public Task<IResourceFactory<Stream>> CreateAsync(ResourceFactoryCreateContext<Stream> context)
        {
            return Task.FromResult<IResourceFactory<Stream>>(new ResourceFactory(context.SourceProvider));
        }

        public void Dispose()
        {
        }
        class ResourceFactory : IResourceFactory<Stream>
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
