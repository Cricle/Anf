using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Easy.Store
{
    public class FileStoreService : IStoreService, IComicSaver
    {
        public const string DefaultFolderName = "Stores";

        public const int DefaultCacheSize = 50;

        private readonly IAddressToFileNameProvider addressToFileNameProvider;
        private readonly LruCacher<string, FileInfo> addressToFileMap;
        private readonly LruCacher<string, DirectoryInfo> domainFolders;

        public FileStoreService(DirectoryInfo folder,
            IAddressToFileNameProvider addressToFileNameProvider,
            int cacheSize = DefaultCacheSize)
        {
            Folder = folder;
            PathHelper.EnsureCreated(folder.FullName);
            addressToFileMap = new LruCacher<string, FileInfo>(cacheSize);
            domainFolders = new LruCacher<string, DirectoryInfo>(cacheSize);
            this.addressToFileNameProvider = addressToFileNameProvider ?? throw new ArgumentNullException(nameof(addressToFileNameProvider));
        }

        public DirectoryInfo Folder { get; }

        public virtual string GetFileName(string str)
        {
            return addressToFileNameProvider.Convert(str);
        }

        private DirectoryInfo EnsureGetFolder(string address)
        {
            var host = UrlHelper.FastGetHost(address);
            host = PathHelper.EnsureName(host);
            var folder = domainFolders.GetOrAdd(host, () =>
            {
                var path = Path.Combine(Folder.FullName, host);
                var dirInfo = new DirectoryInfo(path);
                if (!dirInfo.Exists)
                {
                    dirInfo.Create();
                }
                return dirInfo;
            });
            return folder;
        }
        private FileInfo GetFile(string address)
        {
            var key = GetFileName(address);
            var fp = addressToFileMap.Get(key);
            if (fp is null)
            {
                var folder = EnsureGetFolder(address);
                fp = folder.EnumerateFiles(key).FirstOrDefault();
                if (fp is null)
                {
                    fp = new FileInfo(Path.Combine(folder.FullName, key));
                }
                addressToFileMap.Add(key, fp);
            }
            return fp;
        }
        public virtual Task<bool> ExistsAsync(string address)
        {
            return Task.FromResult(GetFile(address).Exists);
        }

        public virtual Task<string> GetPathAsync(string address)
        {
            return Task.FromResult(GetFile(address)?.FullName);
        }

        public virtual async Task<string> SaveAsync(string address, Stream stream)
        {
            var file = GetFile(address);
            using (var fs = OpenFile(file))
            {
                await WriteFileAsync(fs, stream);
                await fs.FlushAsync();
                addressToFileMap.Add(address, file);
                return file.FullName;
            }
        }
        protected virtual Stream OpenFile(FileInfo file)
        {
            return file.Open(FileMode.Create);
        }
        protected virtual Task WriteFileAsync(Stream fs,Stream source)
        {
            return source.CopyToAsync(fs);
        }

        public virtual void Dispose()
        {
            addressToFileNameProvider.Dispose();
        }

        public static FileStoreService FromDefault(string path,
            IAddressToFileNameProvider addressToFileNameProvider,
            int cacheSize = DefaultCacheSize)
        {
            return new FileStoreService(new DirectoryInfo(
                Path.Combine(path, DefaultFolderName)), addressToFileNameProvider,
                cacheSize);
        }
        public static FileStoreService FromMd5Default(string path,int cacheSize=DefaultCacheSize)
        {
            return FromDefault(path, MD5AddressToFileNameProvider.Instance, cacheSize);
        }

        public bool NeedToSave(ComicDownloadContext context)
        {
            var exists = GetFile(context.Page.TargetUrl).Exists;
            return !exists;
        }

        public Task SaveAsync(ComicDownloadContext context)
        {
            return SaveAsync(context.Page.TargetUrl, context.SourceStream);
        }

        public Task<Stream> GetStreamAsync(string address)
        {
            var fileInfo = GetFile(address);
            if (fileInfo is null)
            {
                return Task.FromResult<Stream>(null);
            }
            return Task.FromResult<Stream>(fileInfo.OpenRead());
        }

        protected virtual FileInfo DecorateFileInfo(FileInfo file)
        {
            return file;
        }
    }
}
