using Kw.Comic.Wpf.Managers;
using Kw.Comic.Wpf.Views.Pages;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using Microsoft.Extensions.DependencyInjection;
using SourceChord.FluentWPF;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Navigation;

namespace Kw.Comic.Wpf.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        public MainWindow()
        {
            InitializeComponent();
            var navSer = WpfAppEngine.Instance.GetRequiredService<MainNavigationService>();
            var barSer = WpfAppEngine.Instance.GetRequiredService<CommandBarManager>();
            LeftCommands.ShowSeparators =
                RightCommands.ShowSeparators = false;
            LeftCommands.ShowLastSeparator =
                RightCommands.ShowLastSeparator = true;
            LeftCommands.ItemsSource = barSer.LeftCommands;
            RightCommands.ItemsSource = barSer.RightCommands;

            navSer.Frame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            Content = navSer.Frame;
            var BackBtn = new Button
            {
                Content = new PackIconMaterialLight { Kind = PackIconMaterialLightKind.ChevronLeft }
            };
            BackBtn.SetBinding(Button.IsEnabledProperty, new Binding(nameof(Frame.CanGoBack)) { Source = navSer.Frame });
            BackBtn.Click += (_, __) =>
            {
                if (navSer.Frame.CanGoBack)
                {
                    navSer.Frame.GoBack();
                }
            };
            
            barSer.LeftCommands.Add(BackBtn);
            //navSer.Frame.Navigate(new HomePage());
            navSer.Frame.Navigate(new ViewPage("http://www.dm5.com/manhua-monvzhilv/"));
        }
    }
}
