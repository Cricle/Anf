using System;
using System.IO;
using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy.Visiting
{
    public interface IResourceFactory<TResource> : IDisposable
    {
        Task<TResource> GetAsync(string address);
    }
}
