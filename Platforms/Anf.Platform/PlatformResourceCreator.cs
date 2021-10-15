using Anf.Easy.Visiting;
using Anf.Platform.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Platform
{
    public class PlatformResourceCreator<TResource,TImage> : IResourceFactory<TImage>
    {

        private readonly IComicSourceProvider provider;

        public PlatformResourceCreator(IComicSourceProvider provider)
        {
            this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public void Dispose()
        {
        }

        public bool EnableCache { get; set; } = true;

        public async Task<TImage> GetAsync(string address)
        {
            if (EnableCache)
            {
                var bitmap = await StoreFetchHelper.GetOrFromCacheAsync<TResource,TImage>(address, () => provider.GetImageStreamAsync(address));
                return bitmap;
            }
            else
            {
                var convert = AppEngine.GetRequiredService<IStreamImageConverter<TImage>>();
                using (var mem = await provider.GetImageStreamAsync(address))
                {
                    return await convert.ToImageAsync(mem);
                }
            }
        }
    }
}
