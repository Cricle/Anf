using Kw.Comic.Uwp.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Kw.Comic.Uwp.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ViewPage : Page
    {
        private ViewViewModel vm;
        public ViewPage()
        {
            this.InitializeComponent();
            if (vm==null)
            {
                vm = DataContext as ViewViewModel;
            }
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is string str)
            {
                try
                {
                    vm = await ViewViewModel.FromUriAsync(str);
                    vm.CurrentComicVisitor = vm.ComicVisitors.FirstOrDefault();

                    DataContext = vm;
                }
                catch (Exception ex)
                {
                    await new MessageDialog(ex.Message)
                        .ShowAsync();
                }
            }
            base.OnNavigatedTo(e);
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
