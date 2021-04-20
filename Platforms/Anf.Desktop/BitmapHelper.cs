using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Desktop
{
    internal static class BitmapHelper
    {
        public static async Task<string> PickSaveAsync(this Bitmap bitmap,string name)
        {
            var win = AppEngine.GetRequiredService<MainWindow>();
            var mgr = AppEngine.GetRequiredService<RecyclableMemoryStreamManager>();
            var dig = new SaveFileDialog();
            dig.InitialFileName = name;
            var res = await dig.ShowAsync(win);
            if (res != null && res != null)
            {
                using (var stream = mgr.GetStream())
                {
                    bitmap.Save(stream);
                    stream.Seek(0, SeekOrigin.Begin);

                    using (var fs = File.Open(res, FileMode.Create))
                    {
                        await stream.CopyToAsync(fs);
                    }
                }
            }
            return res;
        }
    }
}
