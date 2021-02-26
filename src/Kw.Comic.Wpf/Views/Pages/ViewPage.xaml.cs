using Kw.Comic.Wpf.Managers;
using Kw.Comic.Wpf.ViewModels;
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

        private void ViewPage_Unloaded(object sender, RoutedEventArgs e)
        {
            var val = WpfAppEngine.Instance.GetRequiredService<CommandBarManager>();
            foreach (var item in controls)
            {
                val.RightCommands.Remove(item);
            }
        }
        private ViewViewModel vm;
        private async void ViewPage_Loaded(object sender, RoutedEventArgs e)
        {
            vm = await ViewViewModel.FromUriAsync(Uri);
            DataContext = vm;
            vm.CurrentComicVisitor = vm.ComicVisitors.FirstOrDefault();
            var val = WpfAppEngine.Instance.GetRequiredService<CommandBarManager>();
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
            val.RightCommands.Add(leftChapter);
            val.RightCommands.Add(rightChapter);
        }

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            vm?.ToggleControlVisibility();
        }
    }
}
