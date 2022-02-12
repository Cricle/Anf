using Anf.Easy;
using Anf.Easy.Store;
using Azure.Storage.Blobs;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Anf.Web
{
    public class AzureStoreService : IStoreService, IComicSaver
    {
        public static readonly string ContainerName = "cache-";

        private readonly BlobServiceClient blobServiceClient;
        private readonly IAddressToFileNameProvider addressToFileNameProvider;
        private readonly LruCacher<string, bool> createdCacher;

        public AzureStoreService(BlobServiceClient blobServiceClient,
            IAddressToFileNameProvider addressToFileNameProvider,
            int cacheSize = FileStoreService.DefaultCacheSize)
        {
            this.blobServiceClient = blobServiceClient;
            createdCacher = new LruCacher<string, bool>(cacheSize);
            this.addressToFileNameProvider = addressToFileNameProvider ?? throw new ArgumentNullException(nameof(addressToFileNameProvider));
        }
        public virtual string GetFileName(string str)
        {
            return addressToFileNameProvider.Convert(str);
        }
        private async Task<BlobClient> GetFileAsync(string address)
        {
            var hash = ContainerName + Md5Helper.MakeMd5(address);
            var client = blobServiceClient.GetBlobContainerClient(ContainerName);
            if (!createdCacher.Get(hash))
            {
                await client.CreateIfNotExistsAsync();
                createdCacher.Add(hash, true);
            }
            var key = GetFileName(address);
            return client.GetBlobClient(key);
        }

        public void Dispose()
        {
        }

        public async Task<bool> ExistsAsync(string address)
        {
            var client = await GetFileAsync(address);
            var rep=await client.ExistsAsync();
            return rep.Value;
        }

        public async Task<string> GetPathAsync(string address)
        {
            var client = await GetFileAsync(address);
            return client.Uri.AbsoluteUri;
        }

        public async Task<Stream> GetStreamAsync(string address)
        {
            var client = await GetFileAsync(address);
            var rep=await client.DownloadAsync();
            return rep.Value.Content;
        }

        public bool NeedToSave(ComicDownloadContext context)
        {
            var exists = ExistsAsync(context.Page.TargetUrl).GetAwaiter().GetResult();
            return !exists;
        }

        public async Task<string> SaveAsync(string address, Stream stream)
        {
            var client = await GetFileAsync(address);
            await client.UploadAsync(stream);
            return client.Uri.AbsoluteUri;
        }

        public Task SaveAsync(ComicDownloadContext context)
        {
            return SaveAsync(context.Page.TargetUrl, context.SourceStream);
        }
    }
}
