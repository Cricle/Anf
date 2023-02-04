using Avalonia.Media.Imaging;
using BetterStreams;
using System.IO;
using System.Threading.Tasks;
using ValueBuffer;

namespace Anf.Desktop.Services
{
    internal class StreamImageConverter : IStreamImageConverter<Bitmap>
    {
        public Task<Bitmap> ToImageAsync(Stream stream)
        {
            if (stream.CanSeek)
            {
                return Task.FromResult(new Bitmap(stream));
            }
            using (var mem = new PooledMemoryStream())
            {
                stream.CopyTo(mem);
                mem.Position = 0;
                return Task.FromResult(new Bitmap(mem));
            }
        }
    }
}
