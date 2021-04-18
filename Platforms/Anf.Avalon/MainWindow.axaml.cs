using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Anf.Avalon.Views;
using System;
using Avalonia.Platform;
using Avalonia.Media;
using Anf.Avalon.Services;
using Anf.Services;
using Avalonia.LogicalTree;
using Avalonia.Data;
using Avalonia.Input;

namespace Anf.Avalon
{
    public class MainWindow : Window, IStyleable
    {
        internal void SetPseudoClasses(string name,bool value)
        {
            PseudoClasses.Set(name, value);
        }
        internal void RemovePseudoClasses(string name)
        {
            PseudoClasses.Remove(name);
        }
        public MainWindow()
        {
            InitializeComponent();
            MinWidth = 850;
            MinHeight = 400;
#if DEBUG&&NET472
            this.AttachDevTools();
#endif
        }
        private Panel mainPlan;
        private MainNavigationService navSer;
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            navSer = (MainNavigationService)AppEngine.GetRequiredService<INavigationService>();
            mainPlan = this.Get<Panel>("MainPlan");
            mainPlan.Children.Add(navSer.border);
            var titleBar = this.Get<Border>("TitleBar");
            var titleSer = AppEngine.GetRequiredService<TitleService>();
            titleBar.DataContext =titleSer;
            titleBar.Bind(HeightProperty, new Binding(nameof(TitleService.OffsceneHeight)) { Source = titleSer });
            exSer = AppEngine.GetRequiredService<ExceptionService>();
            var exBorder = this.Get<Border>("ExcetionBorder");
            exBorder.DataContext = exSer;
            exBorder.KeyDown += ExBorder_KeyDown;
        }
        private ExceptionService exSer;
        private void ExBorder_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key== Key.Escape)
            {
                exSer.Exception = null;
                e.Handled = true;
            }
        }

        protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromLogicalTree(e);
            mainPlan.Children.Remove(navSer.border);   
        }
        Type IStyleable.StyleKey => typeof(Window);


        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.PreferSystemChrome | ExtendClientAreaChromeHints.OSXThickTitleBar;
        }
    }
}
