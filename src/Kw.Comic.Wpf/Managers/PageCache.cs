using Kw.Comic.Managers;
using Kw.Comic.Visit;
using System.Windows.Media.Imaging;

namespace Kw.Comic.Wpf.Managers
{
    public class PageCache : SimpleCacher<PageCursorIndex, BitmapImage>
    {
        public PageCache(int max = 5) 
            : base(max)
        {

        }
    }
}
