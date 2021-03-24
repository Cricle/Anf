using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy.Store
{
    public interface IStoreService : IDisposable
    {
        Task<bool> ExistsAsync(string address);

        Task<string> GetPathAsync(string address);

        Task<Stream> GetStreamAsync(string address);

        Task<string> SaveAsync(string address, Stream stream);
    }
}
