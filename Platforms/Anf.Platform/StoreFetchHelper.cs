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
        public static async Task<Stream> GetOrFromCacheAsync(string address, Func<Task<Stream>> streamCreator, bool notUseCache = false)
        {
            var storeService = AppEngine.GetRequiredService<IStoreService>();
            Stream stream = null;
            if (!notUseCache)
            {
                var str = await storeService.GetPathAsync(address);
                if (File.Exists(str))
                {
                    stream = File.OpenRead(str);
                    return stream;
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
                    stream = await GetOrFromCacheAsync(address, streamCreator, true);
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
