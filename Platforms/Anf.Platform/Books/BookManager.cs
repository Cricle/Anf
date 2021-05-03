using Anf.Easy;
using Anf.Easy.Store;
using Anf.Platform.Stores;
using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Platform.Books
{
    public class BookManager : IStoreService
    {
        public BookManager(DirectoryInfo folder, RecyclableMemoryStreamManager streamManager)
        {
            this.streamManager = streamManager;
            Folder = folder;
            PathHelper.EnsureCreated(folder.FullName);
        }
        private readonly RecyclableMemoryStreamManager streamManager;

        public DirectoryInfo Folder { get; }

        public void Dispose()
        {

        }
        public Task<bool> ExistsAsync(string address)
        {
            var path = GetPath(address);
            var exists = File.Exists(path);
            return Task.FromResult(exists);
        }

        public Task<string> GetPathAsync(string address)
        {
            return Task.FromResult(GetPath(address));
        }

        public Task<Stream> GetStreamAsync(string address)
        {
            var path = GetPath(address);
            if (File.Exists(path))
            {
                return Task.FromResult<Stream>(File.Open(address, FileMode.Open));
            }
            return Task.FromResult<Stream>(null);
        }

        public async Task<string> SaveAsync(string address, Stream stream)
        {
            var path = GetPath(address);
            using (var fs = File.Open(path, FileMode.Create))
            {
                await stream.CopyToAsync(fs);
            }
            return path;
        }
        public async Task<Stream> StoreStreamAsync(string address)
        {
            var stream = streamManager.GetStream();
            var zip = new ZipArchive(stream, ZipArchiveMode.Update, true);
            using (var zipSave = new ZipStoreService(zip))
            {
                var defaultStore = AppEngine.GetRequiredService<IStoreService>();
                var keeper = ComicKeeper.FromDefault(address, zipSave, defaultStore);
                await keeper.StoreAsync();

            }
            await stream.FlushAsync();
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected string GetName(string address)
        {
            return PathHelper.EnsureName(address);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected string GetPath(string address)
        {
            var name = GetName(address);
            return Path.Combine(Folder.FullName, name);
        }
    }
}
