using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Anf.Avalon.Services;
using System;

namespace Anf.Avalon.Views
{
    public class ComicView : UserControl
    {
        private readonly TitleService titleService;
        private readonly IDisposable binder;
        public ComicView()
        {
            InitializeComponent();
            titleService = AppEngine.GetRequiredService<TitleService>();
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
