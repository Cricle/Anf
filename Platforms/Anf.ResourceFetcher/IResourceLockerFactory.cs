using System.Threading.Tasks;

namespace Anf.ResourceFetcher
{
    public interface IResourceLockerFactory
    {
        IResourceLocker CreateLocker(string resource);

        Task<IResourceLocker> CreateLockerAsync(string resource);
    }
}
