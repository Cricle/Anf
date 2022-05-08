using Anf.Services;
using Anf.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Anf.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class VisitingView : Page
    {
        public VisitingView(string address)
        {
            this.InitializeComponent();
            LoadVm(address);
            Sv.ViewChanged += Sv_ViewChanged;

        }

        private async void Sv_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (!vm.ReadingSettings.LoadAll)
            {
                var sv = (ScrollViewer)sender;
                if (sv.ExtentHeight <= sv.VerticalOffset + sv.ViewportHeight * 2)
                {
                    var idx = vm.CurrentPageCursor.CurrentIndex;
                    await vm.LoadResourceAsync(idx + 1);
                    await vm.GoPageIndexAsync(idx + 1);
                }
            }
        }

        UnoVisitingViewModel vm;
        private async void LoadVm(string address)
        {
            vm = new UnoVisitingViewModel();
            DataContext = vm;
            var titleBar = new VisitingControlView { DataContext = vm };
            var appBar = AppEngine.GetRequiredService<AppBarService>();
            appBar.GetAsDefault()
                .Lefts.Add(titleBar);

            try
            {
                await vm.InitAsync(address);
            }
            catch (Exception ex)
            {
                vm.ExceptionService.Exception = ex;
            }
        }
        
    }
}
