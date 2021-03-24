using Kw.Comic.Engine.Easy.Visiting;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Kw.Comic.Wpf.Managers
{
    internal class WpfResourceFactoryCreator : IResourceFactoryCreator<ImageSource>
    {
        public Task<IResourceFactory<ImageSource>> CreateAsync(ResourceFactoryCreateContext<ImageSource> context)
        {
            return Task.FromResult<IResourceFactory<ImageSource>>(
                new WpfResourceFactory(context));
        }
    }
}
