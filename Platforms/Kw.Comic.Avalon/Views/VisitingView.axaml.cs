using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Kw.Comic.Avalon.ViewModels;

namespace Kw.Comic.Avalon.Views
{
    public class VisitingView : UserControl
    {
        public VisitingView()
        {
            InitializeComponent();
            LoadVm();
        }
        private async void LoadVm()
        {
            var vm = await AvalonVisitingViewModel.CreateAsync("https://manhua.dmzj.com/waixingmonv");
            await vm.NextChapterAsync();
            await vm.NextPageAsync();
            DataContext = vm;
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
