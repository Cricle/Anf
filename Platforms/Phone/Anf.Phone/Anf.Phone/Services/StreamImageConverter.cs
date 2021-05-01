using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Anf.Phone.Services
{
    internal class StreamImageConverter : IStreamImageConverter<ImageSource>
    {
        public Task<ImageSource> ToImageAsync(Stream stream)
        {
            var img = ImageSource.FromStream(() => stream);
            return Task.FromResult(img);
        }
    }
}
