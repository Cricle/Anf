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
        private IAppBar appBar = new DefaultAppBar();

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

        public DefaultAppBar()
        {
            Lefts = new ObservableCollection<object>();
            Rights = new ObservableCollection<object>();

            var root = new Grid { Background=new SolidColorBrush(Colors.Transparent)};
            root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(48, GridUnitType.Pixel) });
            root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
            root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
            root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
            root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var cc = CreateContent(nameof(Icon));
            Grid.SetColumn(cc, 1);
            root.Children.Add(cc);

            cc = CreateContent(nameof(Title));
            Grid.SetColumn(cc, 2);
            root.Children.Add(cc);

            var ics = CreateItems(nameof(Lefts));
            Grid.SetColumn(ics, 3);
            root.Children.Add(ics);

            cc = CreateContent(nameof(Search));
            Grid.SetColumn(cc, 4);
            root.Children.Add(cc);

            ics = CreateItems(nameof(Rights));
            Grid.SetColumn(ics, 5);
            root.Children.Add(ics);

            Root = root;
        }
        private ContentControl CreateContent(string name)
        {
            var cc = new ContentControl();
            cc.SetBinding(ContentControl.ContentProperty, new Binding
            {
                Path = new PropertyPath(name),
                Source = this
            });
            return cc;
        }
        private ItemsControl CreateItems(string name)
        {
            var cc = new ItemsControl();
            cc.SetBinding(ItemsControl.ItemsSourceProperty, new Binding
            {
                Path = new PropertyPath(name),
                Source = this
            });
            return cc;
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
