using Anf.Easy.Visiting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Anf.Web
{
    internal class WebResourceFactoryCreator : IResourceFactoryCreator<Stream>
    {
        public Task<IResourceFactory<Stream>> CreateAsync(ResourceFactoryCreateContext<Stream> context)
        {
            return Task.FromResult<IResourceFactory<Stream>>(new WebResourceFactory(context));
        }
    }
}
