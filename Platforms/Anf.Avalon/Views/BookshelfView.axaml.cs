using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Anf.Avalon.Services;
using System;
using Anf.ViewModels;

namespace Anf.Avalon.Views
{
    public class BookshelfView : UserControl
    {
        private readonly IDisposable binder;
        private readonly BookshelfViewModel vm;
        public BookshelfView()
        {
            InitializeComponent();
            DataContext = vm = new BookshelfViewModel();
            binder = AppEngine.GetRequiredService<MainWindow>().BindDecorationMargin(this);
        }
        protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
        {
            binder.Dispose();
            base.OnDetachedFromLogicalTree(e);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
