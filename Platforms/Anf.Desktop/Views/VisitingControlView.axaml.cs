using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Anf.Desktop.Services;
using Anf.Desktop.ViewModels;
using Anf.Services;
using System;

namespace Anf.Desktop.Views
{
    public class VisitingControlView : StackPanel
    {
        private readonly TitleService titleService;
        public VisitingControlView()
        {
            InitializeComponent();
            titleService = AppEngine.GetRequiredService<TitleService>();
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            foreach (var item in Children)
            {
                if (item is Button btn)
                {
                    btn.Bind(TemplatedControl.FontSizeProperty, new Binding(nameof(TitleService.AdviseFontSize)) { Source = titleService });
                }
            }
        }
    }
}
