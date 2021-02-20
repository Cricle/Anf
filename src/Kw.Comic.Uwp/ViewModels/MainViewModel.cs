using GalaSoft.MvvmLight;
using Kw.Comic.Uwp.Managers;
using Kw.Comic.Uwp.Models;
using Kw.Comic.Uwp.Pages;
using MahApps.Metro.IconPacks;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Uwp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            MenuItems = new ObservableCollection<ComicMenuItem>
            {
                new ComicMenuItem(new PathIconJamIcons{Kind= PackIconJamIconsKind.Home},"Home",typeof(HomePage)),
                //new ComicMenuItem(new PathIconJamIcons{Kind= PackIconJamIconsKind.Box},"Bookshelf",typeof(BookshelfPage)),
                //new ComicMenuItem(new PathIconJamIcons{Kind= PackIconJamIconsKind.Download},"Download",typeof(DownloadPage)),
            };
            MenuVisitStack = new Stack<ComicMenuItem>();
            ComicViewManager = UwpAppEngine.Instance.GetRequiredService<ComicViewManager>();
        }
        private ComicMenuItem currentMenuItem;

        public ComicMenuItem CurrentMenuItem
        {
            get { return currentMenuItem; }
            set
            {
                Set(ref currentMenuItem, value);
                if (!IgnoreVisitStack)
                {
                    MenuVisitStack.Push(value);
                }
                MenuItemChanged?.Invoke(value);
            }
        }
        public ComicViewManager ComicViewManager { get; }

        public Stack<ComicMenuItem> MenuVisitStack { get; }

        public ObservableCollection<ComicMenuItem> MenuItems { get; }

        public bool IgnoreVisitStack { get; set; }

        public event Action<ComicMenuItem> MenuItemChanged;

    }
}
