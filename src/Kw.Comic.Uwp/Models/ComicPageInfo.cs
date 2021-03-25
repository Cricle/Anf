using GalaSoft.MvvmLight;
using Kw.Comic.Engine.Easy.Visiting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace Kw.Comic.Uwp.Models
{
    public class ComicPageInfo : ObservableObject
    {

        private IComicVisitPage<ImageSource> page;
        private bool loading;

        public ComicPageInfo(PageSlots<ImageSource> pageSlots, int index)
        {
            PageSlots = pageSlots;
            Index = index;
        }

        public bool Loading
        {
            get { return loading; }
            set => Set(ref loading, value);
        }

        public IComicVisitPage<ImageSource> Page
        {
            get { return page; }
            set => Set(ref page, value);
        }

        public int Index { get; }

        public PageSlots<ImageSource> PageSlots { get; }

        public async Task LoadAsync()
        {
            if (Page == null)
            {
                Loading = true;
                try
                {
                    Page = await PageSlots.GetAsync(Index);
                }
                finally
                {
                    Loading = false;
                }
            }
        }
    }
}
