using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Anf
{
    public interface IStreamImageConverter<TImage>
    {
        Task<TImage> ToImageAsync(Stream stream);
    }
}
