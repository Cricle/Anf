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
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            var win = ApplicationView.GetForCurrentView();
        }

        UnoVisitingViewModel vm;
        private async void LoadVm(string address)
        {
            vm = new UnoVisitingViewModel();
            DataContext = vm;
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
