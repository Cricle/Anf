using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Anf.Desktop.Services;
using System;
using Anf.ViewModels;
using Anf.Desktop.ViewModels;

namespace Anf.Desktop.Views
{
    public class BookshelfView : UserControl
    {
        private readonly IDisposable binder;
        private readonly DesktopBookshelfViewModel vm;
        public BookshelfView()
        {
            InitializeComponent();
            DataContext = vm = new DesktopBookshelfViewModel();
            binder = AppEngine.GetRequiredService<MainWindow>().BindDecorationMargin(this);
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        protected override async void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            await vm.UpdateAsync();
        }
    }
}
