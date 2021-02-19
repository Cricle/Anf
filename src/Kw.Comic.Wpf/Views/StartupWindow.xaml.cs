using Kw.Comic.Wpf.Converters;
using Kw.Comic.Wpf.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using SourceChord.FluentWPF;
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
using System.Windows.Shapes;

namespace Kw.Comic.Wpf.Views
{
    /// <summary>
    /// StartupPage.xaml 的交互逻辑
    /// </summary>
    public partial class StartupWindow : AcrylicWindow
    {
        public StartupWindow()
        {
            InitializeComponent();
            //DataContext = AppEngine.Instance.HostWraper.Services.GetRequiredService<StartupViewModel>();
            //StringImageSourceConvert
            var vm = new StartupViewModel();
            DataContext = vm;
            Unloaded += (_, __) => vm.Dispose();
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                vm.Address = args[1];
                vm.LoadCommand.Execute(null);
            }
        }

    }
}
