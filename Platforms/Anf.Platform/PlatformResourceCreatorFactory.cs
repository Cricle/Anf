using Anf.Easy.Visiting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Platform
{
    public class PlatformResourceCreatorFactory<TImage> : IResourceFactoryCreator<TImage>
    {
        public bool EnableCache { get; set; } = true;

        public Task<IResourceFactory<TImage>> CreateAsync(ResourceFactoryCreateContext<TImage> context)
        {
            return Task.FromResult<IResourceFactory<TImage>>(new PlatformResourceCreator<TImage>(context.SourceProvider) 
            {
                EnableCache= EnableCache
            });
        }
    }
}
