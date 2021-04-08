using System;
using System.IO;
using System.Threading.Tasks;

namespace Anf.Easy.Visiting
{
    public interface IResourceFactory<TResource> : IDisposable
    {
        Task<TResource> GetAsync(string address);
    }
}
