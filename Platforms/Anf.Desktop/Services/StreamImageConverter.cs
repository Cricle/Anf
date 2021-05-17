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
            Bitmap bit = null;
            try
            {
                if (stream.CanSeek)
                {
                    bit = new Bitmap(stream);
                }
                else
                {
                    using (var mem = recyclableMemoryStreamManager.GetStream())
                    {
                        await stream.CopyToAsync(mem);
                        mem.Seek(0, SeekOrigin.Begin);
                        bit = new Bitmap(mem);
                    }
                }
            }
            catch (Exception) { }
            return bit;
        }
    }
}
