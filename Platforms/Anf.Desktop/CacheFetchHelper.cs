using Anf.Easy.Store;
using Anf.Platform;
using Avalonia.Media.Imaging;
using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Desktop
{
    internal static class CacheFetchHelper
    {
        public static Task<Bitmap> GetAsBitmapOrFromCacheAsync(string address, Func<Task<Stream>> streamCreator)
        {
            return StoreFetchHelper.GetAsOrFromCacheAsync(address, streamCreator, x => new Bitmap(x));
        }

    }
}
