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
        public static Task<T> GetOrFromCacheAsync<T>(string address,
            Func<Task<Stream>> streamCreator,
            Func<Stream, T> converter)
        {
            return GetOrFromCacheAsync(address, streamCreator, x => Task.FromResult(converter(x)));
        }
        public static Task<T> GetOrFromCacheAsync<T>(string address)
        {
            return GetOrFromCacheAsync<T>(address, async () =>
            {
                var rep = await AppEngine.GetRequiredService<HttpClient>().GetAsync(address);
                return await rep.Content.ReadAsStreamAsync();
            });
        }
        public static Task<T> GetOrFromCacheAsync<T>(string address,
            Func<Task<Stream>> streamCreator)
        {
            var convert = AppEngine.GetRequiredService<IStreamImageConverter<T>>();
            return GetOrFromCacheAsync(address, streamCreator, x => convert.ToImageAsync(x));
        }
        public static async Task<T> GetOrFromCacheAsync<T>(string address,
            Func<Task<Stream>> streamCreator,
            Func<Stream,Task<T>> converter)
        {
            var stream = await GetOrFromCacheAsync(address, streamCreator);
            if (stream is null)
            {
                return default;
            }
            try
            {
                return await converter(stream);
            }
            catch (Exception)
            {
                try
                {
                    stream = await GetOrFromCacheAsync(address, streamCreator, StoreFetchSettings.NoCache);
                    return await converter(stream);
                }
                catch (Exception)
                {
                    return default;
                }
            }
            finally
            {
                stream?.Dispose();
            }
        }

    }
}
