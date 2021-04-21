using Anf.Easy.Store;
using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.IO;
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

        public static async Task<Stream> GetOrFromCacheAsync(string address, Func<Task<Stream>> streamCreator, StoreFetchSettings settings = null)
        {
            var storeService = AppEngine.GetRequiredService<IStoreService>();
            Stream stream = null;
            if (settings is null)
            {
                settings = DefaultStoreFetchSettings;
            }
            if (!settings.ForceNoCache)
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
            using (var mem = await streamCreator())
            {
                stream = recyclableMemoryStreamManager.GetStream();

                await mem.CopyToAsync(stream);
                stream.Seek(0, SeekOrigin.Begin);
                await storeService.SaveAsync(address, stream);

                stream.Seek(0, SeekOrigin.Begin);
            }
            return stream;
        }
        public static Task<T> GetAsOrFromCacheAsync<T>(string address,
            Func<Task<Stream>> streamCreator,
            Func<Stream, T> converter)
        {
            return GetAsOrFromCacheAsync<T>(address, streamCreator, x => Task.FromResult(converter(x)));
        }
        public static async Task<T> GetAsOrFromCacheAsync<T>(string address,
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
