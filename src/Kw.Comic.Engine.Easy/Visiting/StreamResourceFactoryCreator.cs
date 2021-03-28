using System;
using System.IO;
using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy.Visiting
{
    public class StreamResourceFactoryCreator : IResourceFactoryCreator<Stream>
    {
        public static readonly StreamResourceFactoryCreator Default = new StreamResourceFactoryCreator();

        public Task<IResourceFactory<Stream>> CreateAsync(ResourceFactoryCreateContext<Stream> context)
        {
            return Task.FromResult<IResourceFactory<Stream>>(new StreamResourceFactory(context.SourceProvider));
        }

        public void Dispose()
        {
        }
    }
}
