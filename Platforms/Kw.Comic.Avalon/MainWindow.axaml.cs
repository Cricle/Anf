using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Kw.Comic.Avalon.Views;
using System;
using Avalonia.Platform;
using Avalonia.Media;
using Kw.Comic.Avalon.Services;
using Kw.Comic.Services;
using Avalonia.LogicalTree;

namespace Kw.Comic.Avalon
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
#if DEBUG
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
            this.Get<Border>("TitleBar").DataContext = AppEngine.GetRequiredService<TitleService>();
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
