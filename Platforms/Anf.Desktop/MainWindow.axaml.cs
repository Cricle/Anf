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
using Anf.Platform.Services;
using Anf.Desktop.Settings;

namespace Anf.Desktop
{
    public class MainWindow : Window, IStyleable
    {
        internal void SetPseudoClasses(string name, bool value)
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
#if DEBUG && NET461_OR_GREATER
            this.AttachDevTools();
#endif
        }
        private Panel mainPlan;
        private MainNavigationService navSer;
        private ExceptionService exSer;
        Type IStyleable.StyleKey => typeof(Window);

        internal void RunInitAll()
        {

            navSer = (MainNavigationService)AppEngine.GetRequiredService<MainNavigationService>();
            var titleSer = AppEngine.GetRequiredService<TitleService>();
            exSer = AppEngine.GetRequiredService<ExceptionService>();
            var settings = AppEngine.GetRequiredService<AnfSettings>();

            mainPlan = this.Get<Panel>("MainPlan");
            var titleBar = this.Get<Border>("TitleBar");
            var exBorder = this.Get<Border>("ExcetionBorder");

            mainPlan.Children.Add(navSer.border);

            titleBar.DataContext = titleSer;
            titleBar.Bind(HeightProperty, new Binding(nameof(TitleService.OffsceneHeight)) { Source = titleSer });

            exBorder.DataContext = exSer;
            exBorder.KeyDown += ExBorder_KeyDown;

            this.Bind(MinWidthProperty, new Binding(nameof(WindowSettings.MinWidth))
            {
                Source = settings.Window
            });
            this.Bind(MinHeightProperty, new Binding(nameof(WindowSettings.MinHeight))
            {
                Source = settings.Window
            });
            this.Bind(TopmostProperty, new Binding(nameof(WindowSettings.Topmost))
            {
                Source = settings.Window
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

        }
        private void ExBorder_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
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


        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            ExtendClientAreaChromeHints = ExtendClientAreaChromeHints.PreferSystemChrome | ExtendClientAreaChromeHints.OSXThickTitleBar;
        }
    }
}
