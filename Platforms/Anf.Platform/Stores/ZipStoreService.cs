using Anf.Easy.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Platform.Stores
{
    public class ZipStoreService : IStoreService
    {
        private readonly IAddressToFileNameProvider addressToFileNameProvider;
        private readonly ZipArchive archive;

        public ZipStoreService(ZipArchive archive, IAddressToFileNameProvider addressToFileNameProvider = null)
        {
            this.addressToFileNameProvider = addressToFileNameProvider ?? MD5AddressToFileNameProvider.Instance;
            this.archive = archive ?? throw new ArgumentNullException(nameof(archive));
        }

        public IReadOnlyCollection<ZipArchiveEntry> Entries => archive.Entries;

        public void Dispose()
        {
            archive.Dispose();
        }

        private string GetPath(string address)
        {
            return addressToFileNameProvider.Convert(address);
        }

        public Task<bool> ExistsAsync(string address)
        {
            var path = GetPath(address);
            var entity = archive.GetEntry(path);
            return Task.FromResult(entity!=null);
        }

        public Task<string> GetPathAsync(string address)
        {
            var path = GetPath(address);
            return Task.FromResult(path);
        }

        public Task<Stream> GetStreamAsync(string address)
        {
            var path = GetPath(address);
            var entity = archive.GetEntry(path);
            if (entity is null)
            {
                return Task.FromResult<Stream>(null);
            }
            return Task.FromResult(entity.Open());
        }

        public Task<bool> DeleteAsync(string address)
        {
            var path = GetPath(address);
            var entity = archive.GetEntry(path);
            if (entity is null)
            {
                return Task.FromResult(false);
            }
            entity.Delete();
            return Task.FromResult(true);
        }

        public async Task<string> SaveAsync(string address, Stream stream)
        {
            var path = GetPath(address);
            var entity = archive.CreateEntry(path);
            using (var s = entity.Open())
            {
                await stream.CopyToAsync(s);
            }
            return path;
        }
    }
}
