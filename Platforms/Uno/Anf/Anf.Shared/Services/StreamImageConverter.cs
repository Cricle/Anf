using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Anf.Services
{
    public class ImageBox : IDisposable
    {
        public ImageBox(ImageSource image, Stream stream)
        {
            Image = image;
            Stream = stream;
        }

        public ImageSource Image { get; }

        public Stream Stream { get; }

        public void Dispose()
        {
            Stream.Dispose();
#if !WINDOWS_UWP
            if (Image is BitmapImage bi)
            {
                bi.Dispose();
            }
#endif
        }
    }
    internal class StreamImageConverter : IStreamImageConverter<ImageBox>
    {
        public async Task<ImageBox> ToImageAsync(Stream stream)
        {
            var bitmap = new BitmapImage();
            using (var rand = new InMemoryRandomAccessStream())
            {
                await RandomAccessStream.CopyAsync(stream.AsInputStream(), rand);
                rand.Seek(0);
                await bitmap.SetSourceAsync(rand);
                return new ImageBox(bitmap, stream);
            }
        }
    }
}
