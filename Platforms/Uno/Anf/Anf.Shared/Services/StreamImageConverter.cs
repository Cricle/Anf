using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Anf.Services
{
    internal class StreamImageConverter : IStreamImageConverter<ImageSource>
    {
        private readonly RecyclableMemoryStreamManager recyclableMemoryStreamManager;

        public StreamImageConverter(RecyclableMemoryStreamManager recyclableMemoryStreamManager)
        {
            this.recyclableMemoryStreamManager = recyclableMemoryStreamManager;
        }

        public async Task<ImageSource> ToImageAsync(Stream stream)
        {
            var bitmap = new BitmapImage();
            using (var rs = stream.AsRandomAccessStream())
            {
                await bitmap.SetSourceAsync(rs);
            }
            return bitmap;
        }
    }
}
