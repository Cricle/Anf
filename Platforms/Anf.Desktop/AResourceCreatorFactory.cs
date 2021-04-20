using Avalonia.Media.Imaging;
using Anf.Easy.Visiting;
using System.Threading.Tasks;

namespace Anf.Desktop
{
    internal class AResourceCreatorFactory : IResourceFactoryCreator<Bitmap>
    {
        public Task<IResourceFactory<Bitmap>> CreateAsync(ResourceFactoryCreateContext<Bitmap> context)
        {
            return Task.FromResult<IResourceFactory<Bitmap>>(new AResourceCreator(context));
        }
    }
}
