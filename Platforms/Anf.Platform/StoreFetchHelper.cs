using Anf.Easy.Store;
using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Platform
{
    public static class StoreFetchHelper
    {
        private static StoreFetchSettings defaultStoreFetchSettings = StoreFetchSettings.DefaultCache;

        public static StoreFetchSettings DefaultStoreFetchSettings
        {
            get => defaultStoreFetchSettings;
            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException();
                }
                defaultStoreFetchSettings = value;
            }
        }
        public static Task<Stream> GetOrFromCacheAsync(string address, Func<Task<Stream>> streamCreator, StoreFetchSettings settings = null)
        {
            var storeService = AppEngine.GetService<IStoreService>();
            return GetOrFromCacheAsync(storeService, address, streamCreator, settings);
        }
        public static async Task<Stream> GetOrFromCacheAsync(IStoreService storeService,string address, Func<Task<Stream>> streamCreator, StoreFetchSettings settings = null)
        {
            Stream stream = null;
            if (settings is null)
            {
                settings = DefaultStoreFetchSettings;
            }
            if (!settings.ForceNoCache && storeService != null)
            {
                var str = await storeService.GetPathAsync(address);
                if (File.Exists(str))
                {
                    if (settings.ExpiresTime == null ||
                        (DateTime.Now - File.GetLastWriteTime(str)) < settings.ExpiresTime)
                    {
                        stream = File.OpenRead(str);
                        return stream;
                    }
                }
            }
            var recyclableMemoryStreamManager = AppEngine.GetRequiredService<RecyclableMemoryStreamManager>();
            var mem = await streamCreator();
            if (storeService is null)
            {
                return mem;
            }
            using (mem)
            {
                stream = recyclableMemoryStreamManager.GetStream();

                await mem.CopyToAsync(stream);
                stream.Seek(0, SeekOrigin.Begin);
                await storeService.SaveAsync(address, stream);

                stream.Seek(0, SeekOrigin.Begin);
            }
            return stream;
        }
        public static Task<TImage> GetOrFromCacheAsync<TResource,TImage>(string address,
            Func<Task<Stream>> streamCreator,
            Func<Stream, TImage> converter,
            StoreFetchSettings settings = null)
        {
            return GetOrFromCacheAsync<TResource,TImage>(address, streamCreator, x => Task.FromResult(converter(x)), settings);
        }
        public static Task<TImage> GetOrFromCacheAsync<TResource, TImage>(string address,
            StoreFetchSettings settings = null)
        {
            return GetOrFromCacheAsync<TResource,TImage>(address, async () =>
            {
                var rep = await AppEngine.GetRequiredService<HttpClient>().GetAsync(address);
                return await rep.Content.ReadAsStreamAsync();
            },settings);
        }
        public static Task<TImage> GetOrFromCacheAsync<TResource,TImage>(string address,
            Func<Task<Stream>> streamCreator,
            StoreFetchSettings settings = null)
        {
            var convert = AppEngine.GetRequiredService<IStreamImageConverter<TImage>>();
            return GetOrFromCacheAsync<TResource,TImage>(address, streamCreator, x => convert.ToImageAsync(x), settings);
        }
        public static async Task<TImage> GetOrFromCacheAsync<TResource, TImage>(string address,
            Func<Task<Stream>> streamCreator,
            Func<Stream,Task<TImage>> converter,
            StoreFetchSettings settings = null)
        {
            if (settings is null)
            {
                settings = DefaultStoreFetchSettings;
            }
            var image = await GetOrFromCacheAsync(address, streamCreator, settings);
            if (image == null)
            {
                return default;
            }
            try
            {
                return await converter(image);
            }
            catch (Exception)
            {
                try
                {
                    image = await GetOrFromCacheAsync(address, streamCreator, StoreFetchSettings.NoCache);
                    return await converter(image);
                }
                catch (Exception)
                {
                    return default;
                }
            }
            finally
            {
                if (settings.DisposeStream)
                {
                    image?.Dispose();
                }
            }
        }

    }
}
