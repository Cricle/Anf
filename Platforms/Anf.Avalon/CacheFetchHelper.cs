using Anf.Easy.Store;
using Avalonia.Media.Imaging;
using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Avalon
{
    internal static class CacheFetchHelper
    {
        public static async Task<Bitmap> GetAsBitmapOrFromCacheAsync(string address, Func<Task<Stream>> streamCreator)
        {
            var stream = await GetOrFromCacheAsync(address, streamCreator);
            if (stream is null)
            {
                return null;
            }
            try
            {
                return new Bitmap(stream);
            }
            catch (Exception)
            {
                try
                {

                    stream = await GetOrFromCacheAsync(address, streamCreator, true);
                    return new Bitmap(stream);
                }
                catch (Exception) { return null; }
            }
            finally
            {
                stream?.Dispose();
            }
        }
        public static async Task<Stream> GetOrFromCacheAsync(string address, Func<Task<Stream>> streamCreator,bool notUseCache=false)
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

    }
}
