using Kw.Comic.Wpf.Managers;
using Kw.Comic.Wpf.Models;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Kw.Comic.Wpf.Managers
{
    public class GdiComicPageInfo : WpfComicPageInfo<GdiChapterVisitor>
    {
        public GdiComicPageInfo(GdiChapterVisitor visitor) : base(visitor)
        {
        }

        protected override Task<ImageSource> OnLoadResourceAsync(GdiChapterVisitor visitor)
        {
            var soure = Imaging.CreateBitmapSourceFromHBitmap(visitor.Bitmap.GetHbitmap(),
                IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            return Task.FromResult<ImageSource>(soure);
        }
    }
}
