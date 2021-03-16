using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy.Visiting
{
    public interface IResourceFactoryCreator
    {
        Task<IResourceFactory> CreateAsync(ResourceFactoryCreateContext context);
    }
}
