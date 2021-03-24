using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace System.IO
{
    public static class StreamExtensions
    {
        public static BitmapImage AsBitmapImage(this Stream stream, bool seek = false)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            if (!stream.CanRead)
            {
                throw new ArgumentException("Stream can't read!");
            }
            if (seek&&stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
            var bm = new BitmapImage();
            bm.BeginInit();
            bm.CacheOption = BitmapCacheOption.OnLoad;
            bm.StreamSource = stream;
            bm.EndInit();
            return bm;
        }
    }
}
