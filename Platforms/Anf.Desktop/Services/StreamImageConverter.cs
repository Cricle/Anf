using Avalonia.Media.Imaging;
using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Desktop.Services
{
    internal class StreamImageConverter : IStreamImageConverter<Bitmap>
    {
        private readonly RecyclableMemoryStreamManager recyclableMemoryStreamManager;

        public StreamImageConverter(RecyclableMemoryStreamManager recyclableMemoryStreamManager)
        {
            this.recyclableMemoryStreamManager = recyclableMemoryStreamManager;
        }

        public async Task<Bitmap> ToImageAsync(Stream stream)
        {
            if (stream.CanSeek)
            {
                return new Bitmap(stream);
            }
            using (var mem = recyclableMemoryStreamManager.GetStream())
            {
                await stream.CopyToAsync(mem);
                mem.Seek(0, SeekOrigin.Begin);
                return new Bitmap(mem);
            }
        }
    }
}
