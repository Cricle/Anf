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
        private AvalonVisitingViewModel vm;
        private async void LoadVm()
        {
            vm = await AvalonVisitingViewModel.CreateAsync("https://manhua.dmzj.com/waixingmonv");
            await vm.NextChapterAsync();
            await vm.NextPageAsync();
            this.KeyDown += VisitingView_KeyDown;
            DataContext = vm;
        }

        private async void VisitingView_KeyDown(object sender, Avalonia.Input.KeyEventArgs e)
        {
            if (e.Key == Avalonia.Input.Key.Left)
            {
                await vm.PrevChapterAsync();
            }
            else if (e.Key== Avalonia.Input.Key.Right)
            {
                await vm.NextChapterAsync();
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
