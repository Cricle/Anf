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
        private readonly RecyclableMemoryStreamManager streamManager;

        public StreamImageConverter(RecyclableMemoryStreamManager streamManager)
        {
            this.streamManager = streamManager;
        }

        public async Task<ImageBox> ToImageAsync(Stream stream)
        {
            try
            {
                var bitmap = new BitmapImage();
                using (var rand = streamManager.GetStream())
                {
                    await stream.CopyToAsync(rand);
                    rand.Seek(0,  SeekOrigin.Begin);
                    await bitmap.SetSourceAsync(rand);
                    return new ImageBox(bitmap, stream);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
