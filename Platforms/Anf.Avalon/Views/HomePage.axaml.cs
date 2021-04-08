using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Anf.Avalon.Services;
using Anf.Avalon.ViewModels;
using System;

namespace Anf.Avalon.Views
{
    public class HomePage : UserControl
    {
        private readonly IDisposable binder;
        private readonly AvalonHomeViewModel vm = new AvalonHomeViewModel();
        public HomePage()
        {
            InitializeComponent();
            DataContext = vm;
            binder = AppEngine.GetRequiredService<MainWindow>().BindDecorationMargin(this);
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
