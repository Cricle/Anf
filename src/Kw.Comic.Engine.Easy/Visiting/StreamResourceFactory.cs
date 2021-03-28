using System;
using System.IO;
using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy.Visiting
{
    public class StreamResourceFactory : IResourceFactory<Stream>
    {
        private readonly IComicSourceProvider sourceProvider;

        public StreamResourceFactory(IComicSourceProvider sourceProvider)
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
