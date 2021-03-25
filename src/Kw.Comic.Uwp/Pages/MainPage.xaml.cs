using Kw.Comic.Uwp.Managers;
using Kw.Comic.Uwp.Models;
using Kw.Comic.Uwp.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Extensions.DependencyInjection;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Kw.Comic.Uwp.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly MainViewModel vm;
        private readonly FullSceneManager fullSceneManager;
        public MainPage()
        {
            this.InitializeComponent();
            vm = DataContext as MainViewModel;
            fullSceneManager = AppEngine.GetRequiredService<FullSceneManager>();
            fullSceneManager.Poped += FullSceneManager_Poped;
            fullSceneManager.Pushed += FullSceneManager_Pushed;
            Unloaded += (_, __) =>
            {
                fullSceneManager.Poped -= FullSceneManager_Poped;
                fullSceneManager.Pushed -= FullSceneManager_Pushed;
            };

            if (vm != null)
            {
                vm.MenuItemChanged += Vm_MenuItemChanged;
                Unloaded+=(_,__)=> vm.MenuItemChanged -= Vm_MenuItemChanged;
            }
            vm.CurrentMenuItem = vm.MenuItems.First();

        }

        private void FullSceneManager_Pushed(UIElement obj)
        {
            FullGrid.Children.Add(obj);
        }

        private void FullSceneManager_Poped(UIElement obj)
        {
            FullGrid.Children.Remove(obj);
        }

        private void Vm_MenuItemChanged(ComicMenuItem obj)
        {
            MainFrame.Navigate(obj.PageType);
            MainNav.IsBackEnabled = vm.MenuVisitStack.Count > 1;
        }

        private void MainNav_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if (vm.MenuVisitStack.Count > 1)
            {
                vm.MenuVisitStack.Pop();
                vm.CurrentMenuItem = vm.MenuVisitStack.Pop();
            }
        }

        private void Asb_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            try
            {
                //if (vm.ComicViewManager.SourceCondition!=null&&Window.Current.Content is Frame frame)
                //{
                //    frame.Navigate(typeof(ViewPage), vm.ComicViewManager.TargetUrl);
                //}
            }
            catch (Exception)
            {

            }
        }
    }
}
