using Avalonia.Media.Imaging;
using Kw.Comic.Engine.Easy.Visiting;
using System.Threading.Tasks;

namespace Kw.Comic.Avalon
{
    internal class AResourceCreatorFactory : IResourceFactoryCreator<Bitmap>
    {
        public Task<IResourceFactory<Bitmap>> CreateAsync(ResourceFactoryCreateContext<Bitmap> context)
        {
            return Task.FromResult<IResourceFactory<Bitmap>>(new AResourceCreator(context));
        }
    }
}
