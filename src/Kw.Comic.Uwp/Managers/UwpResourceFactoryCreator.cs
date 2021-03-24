using Kw.Comic.Engine.Easy.Visiting;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace Kw.Comic.Uwp.Managers
{
    internal class UwpResourceFactoryCreator : IResourceFactoryCreator<ImageSource>
    {
        public Task<IResourceFactory<ImageSource>> CreateAsync(ResourceFactoryCreateContext<ImageSource> context)
        {
            return Task.FromResult<IResourceFactory<ImageSource>>(new UwpResourceFactory(context));
        }
    }
}
