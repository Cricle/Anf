using Kw.Comic.Engine.Easy;
using Kw.Comic.Engine.Easy.Store;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;

namespace Kw.Comic.Uwp.Managers
{
    internal class UwpStoreService : IStoreService, IComicSaver
    {
        private readonly IAddressToFileNameProvider addressToFileNameProvider;
        private readonly LruCacher<string, StorageFile> addressToFileMap;
        private readonly ConcurrentDictionary<string, StorageFolder> domainFolders;

        public UwpStoreService(StorageFolder folder,
            IAddressToFileNameProvider addressToFileNameProvider,
            int cacheSize)
        {
            Folder = folder;
            this.addressToFileNameProvider = addressToFileNameProvider;
            addressToFileMap = new LruCacher<string, StorageFile>(cacheSize);
        }

        public StorageFolder Folder { get; }


        private static string GetHost(string address) => new Uri(address).Host;

        public virtual string GetFileName(string str)
        {
            return addressToFileNameProvider.Convert(str);
        }


        Task<string> IStoreService.GetPathAsync(string address)
        {
            throw new NotSupportedException();
        }

        private StorageFolder EnsureGetFolder(string address)
        {
            var host = GetHost(address);
            host = PathHelper.EnsureName(host);
            var folder = domainFolders.GetOrAdd(host, x =>
            {
                var dirInfo = Folder.CreateFolderAsync(x, CreationCollisionOption.OpenIfExists).GetResults();
                return dirInfo;
            });
            return folder;
        }
        private async Task<StorageFile> GetFileAsync(string address, bool notFoundNull = false)
        {
            var key = GetFileName(address);
            var fp = addressToFileMap.Get(key);
            if (fp != null)
            {
                return fp;
            }
            var folder = EnsureGetFolder(address);
            var option = notFoundNull ? CreationCollisionOption.FailIfExists : CreationCollisionOption.OpenIfExists;
            try
            {
                fp = await folder.CreateFileAsync(key, option);
                addressToFileMap.Add(key, fp);
                return fp;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public virtual async Task<bool> ExistsAsync(string address)
        {
            var fi = await GetFileAsync(address);
            return fi != null;
        }

        public virtual async Task<string> SaveAsync(string address, Stream stream)
        {
            var file = await GetFileAsync(address);
            using (var fs = await file.OpenAsync(FileAccessMode.ReadWrite))
            using (var randStream = fs.AsStream())
            {
                await stream.CopyToAsync(randStream);
                addressToFileMap.Add(address, file);
                return file.Path;
            }
        }

        public virtual void Dispose()
        {
            addressToFileNameProvider.Dispose();
        }

        public static async Task<UwpStoreService> FromDefaultAsync(string name,
            IAddressToFileNameProvider addressToFileNameProvider,
            int cacheSize = FileStoreService.DefaultCacheSize)
        {
            var folder = await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync(name, CreationCollisionOption.OpenIfExists);
            return new UwpStoreService(folder, addressToFileNameProvider,
                cacheSize);
        }
        public static Task<UwpStoreService> FromMd5DefaultAsync(string name, int cacheSize = FileStoreService.DefaultCacheSize)
        {
            return FromDefaultAsync(name, MD5AddressToFileNameProvider.Instance, cacheSize);
        }

        public bool NeedToSave(ComicDownloadContext context)
        {
            var fi = GetFileAsync(context.Page.TargetUrl,true).GetAwaiter().GetResult();
            return fi == null;
        }

        public Task SaveAsync(ComicDownloadContext context)
        {
            return SaveAsync(context.Page.TargetUrl, context.SourceStream);
        }

        public async Task<Stream> GetStreamAsync(string address)
        {
            var fileInfo =await GetFileAsync(address,true);
            if (fileInfo == null)
            {
                return null;
            }
            return await fileInfo.OpenStreamForReadAsync();
        }
    }
}
