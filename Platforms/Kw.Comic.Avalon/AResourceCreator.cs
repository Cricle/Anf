using Avalonia.Media.Imaging;
using Kw.Comic.Engine.Easy.Visiting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Avalon
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

        public async Task<Bitmap> GetAsync(string address)
        {
            var stream = await context.SourceProvider.GetImageStreamAsync(address);
            if (stream!=null)
            {
                return new Bitmap(stream);
            }
            return null;
        }
    }
}
