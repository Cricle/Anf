using Anf.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Anf.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DefaultAppBarView : Grid
    {
        public DefaultAppBarView()
        {
            this.InitializeComponent();
            appBar = new DefaultAppBar(this, lefts, rights);
            Icon.SetBinding(ContentControl.ContentProperty, new Binding
            {
                Path = new PropertyPath(nameof(DefaultAppBar.Icon)),
                Source = appBar
            });
            Title.SetBinding(ContentControl.ContentProperty, new Binding
            {
                Path = new PropertyPath(nameof(DefaultAppBar.Title)),
                Source = appBar
            });
            Search.SetBinding(ContentControl.ContentProperty, new Binding
            {
                Path = new PropertyPath(nameof(DefaultAppBar.Search)),
                Source = appBar
            });
            Lefts.ItemsSource = lefts;
            Rights.ItemsSource = rights;
        }

        private readonly ObservableCollection<object> lefts = new ObservableCollection<object>();
        private readonly ObservableCollection<object> rights = new ObservableCollection<object>();

        private readonly DefaultAppBar appBar;

        public DefaultAppBar AppBar => appBar;
    }
}
