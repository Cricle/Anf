using Anf.Easy.Visiting;
using System.Threading.Tasks;

namespace Anf.Easy.Test
{
    internal class NullResourceFactory<T> : IResourceFactory<T>
    {
        public void Dispose()
        {
        }

        public async Task<T> GetAsync(string address)
        {
            return default;
        }
    }
}
