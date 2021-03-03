using Kw.Comic.Wpf.ViewModels;
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
    /// HomePage.xaml 的交互逻辑
    /// </summary>
    public partial class HomePage : Page
    {
        private HomeViewModel vm;
        public HomePage()
        {
            DataContext = vm = new HomeViewModel();
            InitializeComponent();
        }
        private bool loaded;
        private void LvMain_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (!loaded)
            {
                loaded = true;
                if (e.OriginalSource is ScrollViewer viewer)
                {
                    viewer.ScrollChanged += Viewer_ScrollChanged;
                    Unloaded+=(_,__)=> viewer.ScrollChanged -= Viewer_ScrollChanged;
                }
            }
        }

        private void Viewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (sender is ScrollViewer viewer)
            {
                if (viewer.VerticalOffset == viewer.ScrollableHeight)
                {
                    vm.Next();
                }
            }
        }
    }
}
