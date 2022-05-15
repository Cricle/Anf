using Anf.Views;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Anf.Services
{
    public interface IAppBar
    {
        UIElement Root { get; }
    }
    public class AppBarService : ObservableObject
    {
        private IAppBar appBar = new DefaultAppBarView().AppBar;

        public IAppBar AppBar
        {
            get => appBar;
            set => SetProperty(ref appBar, value);
        }

        public DefaultAppBar GetAsDefault()
        {
            return appBar as DefaultAppBar;
        }
    }

    public class DefaultAppBar : ObservableObject, IAppBar
    {
        private object icon;
        private object title;
        private object search;

        public DefaultAppBar(UIElement root, ObservableCollection<object> lefts, ObservableCollection<object> rights)
        {
            Root = root;
            Lefts = lefts;
            Rights = rights;
        }

        public object Search
        {
            get => search;
            set => SetProperty(ref search, value);
        }

        public object Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        public object Icon
        {
            get => icon;
            set => SetProperty(ref icon, value);
        }


        public UIElement Root { get; }

        public ObservableCollection<object> Lefts { get; }

        public ObservableCollection<object> Rights { get; }

    }
}
