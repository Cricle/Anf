using Kw.Comic.Wpf.Managers;
using Kw.Comic.Wpf.Models;
using SkiaSharp;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Kw.Comic.Wpf.Managers
{
    public class SkiaComicPageInfo : ComicPageInfo<SkiaChapterVisitor>
    {
        public SkiaComicPageInfo(SkiaChapterVisitor visitor) : base(visitor)
        {
        }

        protected override Task<ImageSource> OnLoadResourceAsync(SkiaChapterVisitor visitor)
        {
            var size = CreateSize(visitor.Image.Width, visitor.Image.Height, out var scaleX, out var scaleY);
            if (size.Width <= 0 || size.Height <= 0)
                return Task.FromResult<ImageSource>(null);

            var info = new SKImageInfo(size.Width, size.Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
            var bitmap = new WriteableBitmap(info.Width, size.Height, 96 * scaleX, 96 * scaleY, PixelFormats.Pbgra32, null);

            // draw on the bitmap
            bitmap.Lock();
            using (var surface = SKSurface.Create(info, bitmap.BackBuffer, bitmap.BackBufferStride))
            {
                surface.Canvas.DrawImage(visitor.Image, SKPoint.Empty);
            }

            // draw the bitmap to the screen
            bitmap.AddDirtyRect(new Int32Rect(0, 0, info.Width, size.Height));
            bitmap.Unlock();
            return Task.FromResult<ImageSource>(bitmap);
        }
    }
}
