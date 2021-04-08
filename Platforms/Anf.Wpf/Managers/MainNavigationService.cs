using Kw.Comic.Models;
using Kw.Comic.Services;
using Kw.Comic.Wpf.Views.Pages;
using Kw.Core.Annotations;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Kw.Comic.Wpf.Managers
{
    public class MainNavigationService : INavigationService,IComicTurnPageService
    {
        public const string AppName = "ANF";
        public Frame Frame { get; } = new Frame();

        public Window Window => App.Current.MainWindow;

        public bool CanGoBack => Frame.CanGoBack;

        public bool CanGoForward => Frame.CanGoForward;

        public bool GoBack()
        {
            if (CanGoBack)
            {
                Frame.GoBack();
            }
            return false;
        }

        public bool GoForward()
        {
            if (CanGoForward)
            {
                Frame.GoForward();
            }
            return false;
        }

        public void GoSource(ComicSourceInfo info)
        {
            var viewPage = new ViewPage(info.Source.TargetUrl);
            Navigate(viewPage);
        }

        public void Navigate(object dest)
        {
            Frame.Navigate(dest);
        }

        public void SetTitle(string info)
        {
            var win = Window;
            if (!string.IsNullOrEmpty(info))
            {
                win.Title = AppName + " - " + info;
            }
            else
            {
                win.Title = AppName;
            }
        }
    }
}
