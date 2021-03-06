using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Kw.Comic.Rendering;
using Kw.Comic.Visit;
using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Kw.Comic.Wpf.Models
{
    public abstract class WpfComicPageInfo<T> : ComicPageInfo<T, ImageSource>
        where T : ChapterVisitorBase
    {
        public WpfComicPageInfo(T visitor,PageCursorBase<T> pageCursor)
            : base(visitor,pageCursor)
        {
            LoadCommand = new RelayCommand(() => _ = LoadAsync());
        }


        public ICommand LoadCommand { get; }

        public bool IgnorePixelScaling { get; set; }

        protected Size CreateSize(double actualWidth, double actualHeight, out double scaleX, out double scaleY)
        {
            scaleX = 1.0;
            scaleY = 1.0;

            var w = actualWidth;
            var h = actualHeight;

            if (!IsPositive(w) || !IsPositive(h))
                return Size.Empty;

            if (IgnorePixelScaling)
                return new Size((int)w, (int)h);

            var m = System.Windows.PresentationSource.FromVisual(App.Current.MainWindow).CompositionTarget.TransformToDevice;
            scaleX = m.M11;
            scaleY = m.M22;
            return new Size((int)(w * scaleX), (int)(h * scaleY));

            bool IsPositive(double value)
            {
                return !double.IsNaN(value) && !double.IsInfinity(value) && value > 0;
            }
        }
    }
}
