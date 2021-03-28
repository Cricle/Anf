using Kw.Comic.Engine.Easy.Visiting;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Kw.Comic
{
    internal class XResourceFactoryCreator : IResourceFactoryCreator<ImageSource>
    {
        public Task<IResourceFactory<ImageSource>> CreateAsync(ResourceFactoryCreateContext<ImageSource> context)
        {
            return Task.FromResult<IResourceFactory<ImageSource>>(new XResourceFactory(context));
        }
    }
}
