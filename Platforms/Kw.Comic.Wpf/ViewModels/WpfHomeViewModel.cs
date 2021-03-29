using GalaSoft.MvvmLight;
using Kw.Comic.Engine;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Kw.Comic.Wpf.Views.Pages;
using GalaSoft.MvvmLight.Command;
using Kw.Comic.Wpf.Managers;
using Kw.Comic.ViewModels;
using Kw.Comic.Models;
using Kw.Comic.Services;

namespace Kw.Comic.Wpf.ViewModels
{
    public class WpfHomeViewModel: HomeViewModel
    {
        public WpfHomeViewModel()
        {
        }
        private Visibility loadingVisibility = Visibility.Collapsed;

        public Visibility LoadingVisibility
        {
            get { return loadingVisibility; }
            private set => Set(ref loadingVisibility, value);
        }
        protected override void OnCurrentComicSnapshotChanged(ComicSnapshotInfo info)
        {
            var navSer = AppEngine.GetRequiredService<INavigationService>();
            navSer.Navigate(new Views.Pages.ComicPage(info));
        }
        protected override void OnSearchingChanged(bool res)
        {
            LoadingVisibility = res ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
