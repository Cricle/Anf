using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy.Store
{
    public class FileStoreSaver : IStoreService,IComicSaver
    {
        public const string DefaultFolderName = "Stores";

        public const int DefaultCacheSize = 50;

        private readonly IAddressToFileNameProvider addressToFileNameProvider;
        private readonly LruCacher<string, FileInfo> addressToFileMap;
        private readonly ConcurrentDictionary<string, DirectoryInfo> domainFolders;

        public FileStoreSaver(DirectoryInfo folder,
            IAddressToFileNameProvider addressToFileNameProvider,
            int cacheSize = DefaultCacheSize)
        {
            Folder = folder;
            PathHelper.EnsureCreated(folder.FullName);
            addressToFileMap = new LruCacher<string, FileInfo>(cacheSize);
            domainFolders = new ConcurrentDictionary<string, DirectoryInfo>();
            this.addressToFileNameProvider = addressToFileNameProvider ?? throw new ArgumentNullException(nameof(addressToFileNameProvider));
        }

        public DirectoryInfo Folder { get; }

        private static string GetHost(string address) => new Uri(address).Host;

        public virtual string GetFileName(string str)
        {
            return addressToFileNameProvider.Convert(str);
        }

        private DirectoryInfo EnsureGetFolder(string address)
        {
            var host = GetHost(address);
            host = PathHelper.EnsureName(host);
            var folder = domainFolders.GetOrAdd(host, x =>
            {
                var path = Path.Combine(Folder.FullName, x);
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
            if (fp != null)
            {
                return fp;
            }
            var folder = EnsureGetFolder(address);
            fp = folder.EnumerateFiles(key).FirstOrDefault();
            if (fp != null)
            {
                addressToFileMap.Add(key, fp);
                return fp;
            }
            return new FileInfo(Path.Combine(folder.FullName, key));
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
            using (var fs = file.Open(FileMode.Create))
            {
                await stream.CopyToAsync(fs);
                addressToFileMap.Add(address, file);
                return file.FullName;
            }
        }

        public virtual void Dispose()
        {
            addressToFileNameProvider.Dispose();
        }

        public static FileStoreSaver FromDefault(string path,
            IAddressToFileNameProvider addressToFileNameProvider,
            int cacheSize = DefaultCacheSize)
        {
            return new FileStoreSaver(new DirectoryInfo(
                Path.Combine(path, DefaultFolderName)), addressToFileNameProvider,
                cacheSize);
        }
        public static FileStoreSaver FromMd5Default(string path,int cacheSize=DefaultCacheSize)
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
    }
}
