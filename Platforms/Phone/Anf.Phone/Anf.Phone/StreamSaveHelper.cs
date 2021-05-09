using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Anf.Phone
{
    internal static class StreamSaveHelper
    {
        public static async Task<string> PickSaveAsync(this Stream stream)
        {
            var dig =await FilePicker.PickAsync();
            if (dig is null)
            {
                return null;
            }
            using (var fs=File.Open(dig.FullPath, FileMode.Create))
            {
                await stream.CopyToAsync(fs);
            }
            return dig.FileName;
        }
    }
}
