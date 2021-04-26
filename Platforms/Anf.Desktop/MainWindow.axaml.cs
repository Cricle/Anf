using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Anf.Desktop.Views;
using System;
using Avalonia.Platform;
using Avalonia.Media;
using Anf.Desktop.Services;
using Anf.Services;
using Avalonia.LogicalTree;
using Avalonia.Data;
using Avalonia.Input;

namespace Anf.Desktop
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
            MinWidth = 650;
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
            var titleSer = AppEngine.GetRequiredService<TitleService>();
            exSer = AppEngine.GetRequiredService<ExceptionService>();

            mainPlan = this.Get<Panel>("MainPlan");
            mainPlan.Children.Add(navSer.border);

            var titleBar = this.Get<Border>("TitleBar");
            titleBar.DataContext =titleSer;
            titleBar.Bind(HeightProperty, new Binding(nameof(TitleService.OffsceneHeight)) { Source = titleSer });
           
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
