using Anf.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
        }

        UnoVisitingViewModel vm;
        private async void LoadVm(string address)
        {
            vm = new UnoVisitingViewModel();
            DataContext = vm;
            try
            {
                await vm.Visiting.LoadAsync(address);
                if (vm.HasStoreBox)
                {
                    await vm.GoChapterIndexAsync(vm.StoreBox.AttackModel.CurrentChapter);
                }
                else
                {
                    await vm.NextChapterAsync();
                }
                //_ = LoadPageAsync(0);
                _ = vm.LoadAllAsync();
            }
            catch (Exception ex)
            {
                vm.ExceptionService.Exception = ex;
            }
        }
        private Task LoadPageAsync(int index)
        {
            var p = vm.GetResource(index);
            if (p is null)
            {
                return Task.CompletedTask;
            }
            return p.LoadAsync();
        }

    }
}
