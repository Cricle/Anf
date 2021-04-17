using System.Threading.Tasks;

namespace Anf.Easy.Visiting
{
    public interface IResourceFactoryCreator<TResource>
    {
        Task<IResourceFactory<TResource>> CreateAsync(ResourceFactoryCreateContext<TResource> context);
    }
}
