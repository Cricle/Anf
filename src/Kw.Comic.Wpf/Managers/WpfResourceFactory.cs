using Kw.Comic.Engine.Easy.Visiting;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Kw.Comic.Wpf.Managers
{
    internal class WpfResourceFactory : IResourceFactory<ImageSource>
    {
        private ResourceFactoryCreateContext<ImageSource> context;

        public WpfResourceFactory(ResourceFactoryCreateContext<ImageSource> context)
        {
            this.context = context;
        }

        public void Dispose()
        {
        }

        public async Task<ImageSource> GetAsync(string address)
        {
            using (var stream = await context.SourceProvider.GetImageStreamAsync(address))
            {
                if (stream != null)
                {
                    var source = new BitmapImage();
                    source.CacheOption = BitmapCacheOption.OnLoad;
                    source.BeginInit();
                    source.StreamSource = stream;
                    source.EndInit();
                    return source;
                }
            }
            return null;
        }
    }
}
