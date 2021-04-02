using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Avalon.Services
{
    internal class StreamImageConverter : IStreamImageConverter<Bitmap>
    {
        public Task<Bitmap> ToImageAsync(Stream stream)
        {
            return Task.FromResult(new Bitmap(stream));
        }
    }
}
