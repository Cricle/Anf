using System;
using System.IO;
using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy.Visiting
{
    public interface ISharedComic : IDisposable
    {
        int? Capacity { get; }

        void Add(string key, Task<Stream> value);

        Task<Stream> GetAsync(string address);

        Stream Get(string address);

        Task<Stream> GetOrLoadAsync(string address);
    }
}
