using Kw.Comic;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Kw.Comic.Wpf
{
    public class WpfStreamImageConverter : IStreamImageConverter<ImageSource>
    {
        public Task<ImageSource> ToImageAsync(Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            if (!stream.CanRead)
            {
                throw new ArgumentException("Stream can't read!");
            }
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
            var bm = new BitmapImage();
            bm.BeginInit();
            bm.CacheOption = BitmapCacheOption.OnLoad;
            bm.StreamSource = stream;
            bm.EndInit();
            return Task.FromResult<ImageSource>(bm);
        }
    }
}
