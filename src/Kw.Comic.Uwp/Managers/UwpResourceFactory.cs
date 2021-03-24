using Kw.Comic.Engine.Easy.Visiting;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Kw.Comic.Uwp.Managers
{
    internal class UwpResourceFactory : IResourceFactory<ImageSource>
    {
        private readonly ResourceFactoryCreateContext<ImageSource> context;

        public UwpResourceFactory(ResourceFactoryCreateContext<ImageSource> context)
        {
            this.context = context;
        }

        public void Dispose()
        {
        }

        public async Task<ImageSource> GetAsync(string address)
        {
            using (var stream = await context.SourceProvider.GetImageStreamAsync(address))
            using (var randStream = stream.AsRandomAccessStream())
            {
                var bitmap = new BitmapImage();
                await bitmap.SetSourceAsync(randStream);
                return bitmap;
            }
        }
    }
}
