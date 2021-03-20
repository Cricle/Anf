using System;
using System.IO;
using System.Threading.Tasks;

namespace KwC.Services
{
    public interface IStoreService : IDisposable
    {
        Task<bool> ExistsAsync(string address);

        Task<string> GetPathAsync(string address);

        Task<string> SaveAsync(string address, Stream stream);
    }
}
