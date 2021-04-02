using Avalonia.Media.Imaging;
using Kw.Comic.Engine.Easy.Store;
using Kw.Comic.Engine.Easy.Visiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Avalon
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
                    stream = await context.SourceProvider.GetImageStreamAsync(address);
                    var mem = recyclableMemoryStreamManager.GetStream();

                    await stream.CopyToAsync(mem);
                    mem.Seek(0, SeekOrigin.Begin);
                    await storeService.SaveAsync(address, mem);

                    mem.Seek(0, SeekOrigin.Begin);
                    stream = mem;
                }
                return new Bitmap(stream);
            }
            finally
            {
                stream.Dispose();
            }
        }
    }
}
