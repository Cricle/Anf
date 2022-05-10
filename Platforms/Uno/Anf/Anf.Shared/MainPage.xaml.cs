using Anf.Services;
using Anf.ViewModels;
using Anf.Views;
using Ao.Lang.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Anf
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            //Loaded += MainPage_Loaded;
            var appBarSer = AppEngine.Provider.GetRequiredService<AppBarService>();
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = false;
            //var tb = ApplicationView.GetForCurrentView().TitleBar;
            //tb.BackgroundColor = Colors.Khaki;
            //tb.ButtonBackgroundColor = Colors.Transparent;
            AppBarContent.SetBinding(ContentControl.ContentProperty, new Binding
            {
                Source = appBarSer,
                Path = new PropertyPath(nameof(AppBarService.AppBar) + "." + nameof(IAppBar.Root))
            });
            var rt = AppEngine.Provider.GetRequiredService<UnoRuntime>();
            Nv.Content = rt.ContentFrame;
            rt.ContentFrame.Content = new HomePage();
            //Window.Current.SetTitleBar(AppBarContent);
            //Nv.Content = new ComicView
            //{
            //    DataContext = new UnoComicViewModel(new ComicSnapshot
            //    {
            //        Author = "丝丝",
            //        Descript = "缓更中orz 微博@有机高分子纤维丝",
            //        ImageUri = "https://fm.soman.com/img/webpic/18/1002281181435917531.jpg",
            //        Name = "啊",
            //        Sources = new ComicSource[]
            //        {
            //            new ComicSource
            //            {
            //                Name="dmzj",
            //                TargetUrl="http://www.dmzj.com/info/huaahuaa.html"
            //            }
            //        },
            //        TargetUrl = "https://api.soman.com/soman.ashx?action=getsomancomics2&pageindex=1&pagesize=50&keyword=啊"
            //    })
            //};
            //Nv.Content = new HomePage { DataContext=new UnoHomeViewModel()};
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Nv.Content = new VisitingView("https://ac.qq.com/Comic/comicInfo/id/536332");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var lang = LanguageManager.Instance;
            if (!lang.SwitchIfNot("zh-cn","en-IE"))
            {
                lang.SwitchIfNot("en-IE", "zh-cn");
            }
        }
    }
}
