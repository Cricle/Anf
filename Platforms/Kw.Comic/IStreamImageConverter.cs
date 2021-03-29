using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic
{
    public interface IStreamImageConverter<TImage>
    {
        Task<TImage> ToImageAsync(Stream stream);
    }
}
