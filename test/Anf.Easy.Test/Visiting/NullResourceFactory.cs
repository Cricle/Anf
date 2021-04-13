using Anf.Easy.Visiting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Anf.Easy.Test.Visiting
{
    internal class NullResourceFactory<T> : IResourceFactory<T>
    {
        public void Dispose()
        {
        }

        public Dictionary<string,T> Values { get; set; }

        public Task<T> GetAsync(string address)
        {
            return Task.FromResult(Values[address]);
        }
    }
}
