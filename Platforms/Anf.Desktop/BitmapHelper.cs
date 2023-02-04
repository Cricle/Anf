using Avalonia.Controls;
using Avalonia.Media.Imaging;
using BetterStreams;
using System.IO;
using System.Threading.Tasks;
using ValueBuffer;

namespace Anf.Desktop
{
    internal static class BitmapHelper
    {
        public static async Task<string> PickSaveAsync(this Bitmap bitmap,string name)
        {
            var win = AppEngine.GetRequiredService<MainWindow>();
            var dig = new SaveFileDialog();
            dig.InitialFileName = name;
            var res = await dig.ShowAsync(win);
            if (res != null)
            {
                using (var stream = new PooledMemoryStream())
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
