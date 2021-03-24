using Kw.Comic.Wpf.Managers;
using Kw.Comic.Wpf.ViewModels;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Kw.Comic.Wpf.Views.Pages
{
    /// <summary>
    /// ViewPage.xaml 的交互逻辑
    /// </summary>
    public partial class ViewPage : Page
    {
        public string Uri { get; set; }

        private readonly List<object> controls = new List<object>();

        public ViewPage(string uri)
        {
            Uri = uri;
            InitializeComponent();
            Loaded += ViewPage_Loaded;
            Unloaded += ViewPage_Unloaded;
        }
        public ViewPage(ViewViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
            Loaded += ViewPage_Loaded;
            Unloaded += ViewPage_Unloaded;
        }

        private void ViewPage_Unloaded(object sender, RoutedEventArgs e)
        {
            var val = AppEngine.GetRequiredService<CommandBarManager>();
            foreach (var item in controls)
            {
                val.RightCommands.Remove(item);
            }
        }
        private ViewViewModel vm;
        private async void ViewPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (vm == null)
            {
                vm = await ViewViewModel.FromUriAsync(Uri);
                DataContext = vm;
            }
            var val = AppEngine.GetRequiredService<CommandBarManager>();
            var leftChapter = new Button
            {
                Content = new PackIconMaterialLight { Kind = PackIconMaterialLightKind.ArrowLeft },
                Command = vm.PrevChapterCommand
            };
            controls.Add(leftChapter);
            var rightChapter = new Button
            {
                Content = new PackIconMaterialLight { Kind = PackIconMaterialLightKind.ArrowRight },
                Command = vm.NextChapterCommand
            };
            controls.Add(rightChapter);
            var sw = new ToggleSwitch
            {
                Width = 80,
                Content = "锁",
                VerticalAlignment = VerticalAlignment.Center
            };
            sw.Toggled += Sw_Toggled;
            sw.SetResourceReference(ToggleSwitch.ForegroundProperty, "MahApps.Brushes.Foreground");
            val.RightCommands.Add(sw);
            val.RightCommands.Add(leftChapter);
            val.RightCommands.Add(rightChapter);
        }
        private bool swEnable;
        private void Sw_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch @switch)
            {
                swEnable = @switch.IsOn;
            }
        }

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!swEnable)
            {
                vm?.ToggleControlVisibility();
            }
        }

        private void Fv_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key== Key.Left)
            {
                vm.PrevPage();
            }
            else if (e.Key== Key.Right)
            {
                vm.NextPage();
            }
            else if (e.Key== Key.Up)
            {
                vm.PrevChapter();
            }
            else if (e.Key== Key.Down)
            {
                vm.NextChapter();
            }
        }
    }
}
