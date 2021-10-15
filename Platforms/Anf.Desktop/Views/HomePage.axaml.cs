using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Anf.Desktop.Services;
using Anf.Desktop.ViewModels;
using System;
using Anf.Engine;

namespace Anf.Desktop.Views
{
    public class HomePage : UserControl
    {
        private readonly DesktopHomeViewModel vm = new DesktopHomeViewModel();
        public HomePage()
        {
            InitializeComponent();
            DataContext = vm;
            AppEngine.GetRequiredService<MainWindow>().BindDecorationMargin(this);
        }
        private void InitializeComponent()
        {
            Run();
            AvaloniaXamlLoader.Load(this);
        }
        private async void Run()
        {
            var eng = AppEngine.GetRequiredService<ProposalEngine>();
            var p = eng.Active(0);
            await p.Provider.GetProposalAsync(10);
        }
    }
}
