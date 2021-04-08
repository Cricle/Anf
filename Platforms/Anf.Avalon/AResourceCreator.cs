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

namespace Anf.Avalon
{
    internal class AResourceCreator : IResourceFactory<Bitmap>
    {
        private readonly IStoreService storeService;
        private readonly RecyclableMemoryStreamManager recyclableMemoryStreamManager;
        private readonly ResourceFactoryCreateContext<Bitmap> context;

        public AResourceCreator(ResourceFactoryCreateContext<Bitmap> context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            storeService = context.Visiting.Host.GetRequiredService<IStoreService>();
            recyclableMemoryStreamManager = context.Visiting.Host.GetRequiredService<RecyclableMemoryStreamManager>();
        }

        public void Dispose()
        {
        }
        public async Task<Bitmap> GetAsync(string address)
        {
#if ENABLE_CACHE
            var str = await storeService.GetPathAsync(address);
            Stream stream = null;
            try
            {
                if (File.Exists(str))
                {
                    stream = File.OpenRead(str);
                }
                else
                {
                    using (var mem = await context.SourceProvider.GetImageStreamAsync(address))
                    {
                        stream = recyclableMemoryStreamManager.GetStream();

                        await mem.CopyToAsync(stream);
                        stream.Seek(0, SeekOrigin.Begin);
                        await storeService.SaveAsync(address, stream);

                        stream.Seek(0, SeekOrigin.Begin);
                    }
                }
                return new Bitmap(stream);
            }
            finally
            {
                stream?.Dispose();
            }
#else
            using (var mem = await context.SourceProvider.GetImageStreamAsync(address))
            {
                return new Bitmap(mem);
            }
#endif
        }
    }
}
