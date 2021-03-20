using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy.Visiting
{
    public interface IResourceFactoryCreator<TResource>
    {
        Task<IResourceFactory<TResource>> CreateAsync(ResourceFactoryCreateContext<TResource> context);
    }
}
