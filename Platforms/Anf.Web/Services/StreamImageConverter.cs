using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Anf.Web.Services
{
    internal class StreamImageConverter : IStreamImageConverter<Stream>
    {
        public Task<Stream> ToImageAsync(Stream stream)
        {
            return Task.FromResult(stream);
        }
    }
}
