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
            Bitmap bit = null;
            try
            {
                bit = new Bitmap(stream);
            }
            catch (Exception) { }
            return Task.FromResult(bit);
        }
    }
}
