using Avalonia.Media.Imaging;
using Anf.Easy.Store;
using Anf.Easy.Visiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;

namespace Anf.Avalon
{
    internal class AResourceCreator : IResourceFactory<Bitmap>
    {
        private readonly ResourceFactoryCreateContext<Bitmap> context;

        public AResourceCreator(ResourceFactoryCreateContext<Bitmap> context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Dispose()
        {
        }

        public bool EnableCache { get; set; }=true;


        public async Task<Bitmap> GetAsync(string address)
        {
            if (EnableCache)
            {
                var bitmap= await CacheFetchHelper.GetAsBitmapOrFromCacheAsync(address, () => context.SourceProvider.GetImageStreamAsync(address));
                return bitmap;
            }
            else
            {
                using (var mem = await context.SourceProvider.GetImageStreamAsync(address))
                {
                    return new Bitmap(mem);
                }
            }
        }
    }
}
