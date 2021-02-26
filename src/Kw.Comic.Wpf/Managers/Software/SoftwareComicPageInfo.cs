using Kw.Comic.Wpf.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Kw.Comic.Wpf.Managers
{
    public class SoftwareComicPageInfo : WpfComicPageInfo<SoftwareChapterVisitor>
    {
        public SoftwareComicPageInfo(SoftwareChapterVisitor visitor) : base(visitor)
        {
            DecodePixelWdith = 0;
            DecodePixelHeight = 0;
        }
        private int decodePixelHeight;
        private int decodePixelWdith;

        public int DecodePixelWdith
        {
            get { return decodePixelWdith; }
            set => RaisePropertyChanged(ref decodePixelWdith, value);
        }

        public int DecodePixelHeight
        {
            get { return decodePixelHeight; }
            set => RaisePropertyChanged(ref decodePixelHeight, value);
        }


        protected override Task<ImageSource> OnLoadResourceAsync(SoftwareChapterVisitor visitor)
        {
#if EnableWin10
            visitor.Image.Seek(0);
#else
            visitor.Image.Seek(0,SeekOrigin.Begin);
#endif
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
#if EnableWin10
            bitmapImage.StreamSource = visitor.Image.AsStream();
#else
            bitmapImage.StreamSource = visitor.Image;
#endif
            bitmapImage.DecodePixelHeight = DecodePixelHeight;
            bitmapImage.DecodePixelWidth = DecodePixelWdith;
            bitmapImage.EndInit();
            bitmapImage.Freeze();
            return Task.FromResult<ImageSource>(bitmapImage);
        }
    }
}
