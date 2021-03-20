using Kw.Comic;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace KwC.Services
{

    public class PhysicalStoreService : IStoreService
    {
        public const string DefaultFolderName = "Stores";

        public const int DefaultCacheSize = 200;

        private readonly MD5CryptoServiceProvider mD5Crypto;
        private readonly LruCacher<string, FileInfo> addressToFileMap;
        private readonly ConcurrentDictionary<string, DirectoryInfo> domainFolders;

        public PhysicalStoreService(DirectoryInfo folder, int cacheSize = DefaultCacheSize)
        {
            Folder = folder;
            PathHelper.EnsureCreated(folder.FullName);
            addressToFileMap = new LruCacher<string, FileInfo>(cacheSize);
            domainFolders = new ConcurrentDictionary<string, DirectoryInfo>();
            mD5Crypto = new MD5CryptoServiceProvider();
        }

        public DirectoryInfo Folder { get; }

        private static string GetHost(string address) => new Uri(address).Host;

        public string GetMD5Hash(string str)
        {
            //就是比string往后一直加要好的优化容器
            var data = mD5Crypto.ComputeHash(Encoding.UTF8.GetBytes(str));
            var length = data.Length;
            var sb = new StringBuilder(length * 2);
            for (int i = 0; i < length; i++)
                sb.Append(data[i].ToString("X2"));
            return sb.ToString();
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
            var key = GetMD5Hash(address);
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
        public Task<bool> ExistsAsync(string address)
        {
            return Task.FromResult(GetFile(address).Exists);
        }

        public Task<string> GetPathAsync(string address)
        {
            return Task.FromResult(GetFile(address)?.FullName);
        }

        public async Task<string> SaveAsync(string address, Stream stream)
        {
            var file = GetFile(address);
            using var fs = file.Open(FileMode.Create);
            await stream.CopyToAsync(fs);
            addressToFileMap.Add(address, file);
            return file.FullName;
        }

        public void Dispose()
        {
            mD5Crypto.Dispose();
        }

        public static PhysicalStoreService FromDefault(string path,int cacheSize= DefaultCacheSize)
        {
            return new PhysicalStoreService(new DirectoryInfo(
                Path.Combine(path, DefaultFolderName)), cacheSize);
        }
        public static PhysicalStoreService FromDefault(int cacheSize = DefaultCacheSize)
        {
            return FromDefault(AppDomain.CurrentDomain.BaseDirectory,cacheSize);
        }
    }
}
