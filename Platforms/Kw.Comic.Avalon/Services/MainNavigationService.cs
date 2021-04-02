using Avalonia.Controls;
using Kw.Comic.Avalon.ViewModels;
using Kw.Comic.Models;
using Kw.Comic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Avalon.Services
{
    internal class MainNavigationService : INavigationService,IComicTurnPageService
    {
        internal readonly Border border;

        public MainNavigationService(Border border)
        {
            this.border = border;
        }

        public bool CanGoBack =>false;

        public bool CanGoForward => false;

        public bool GoBack()
        {
            return false;
        }

        public bool GoForward()
        {
            return false;
        }

        public void GoSource(ComicSourceInfo info)
        {
            //Todo
        }

        public void Navigate(object dest)
        {
            if (dest is IControl control)
            {
                border.Child = control;
            }
            else
            {
                border.Child = new TextBlock { Text = dest?.ToString() };
            }
        }
    }
}
