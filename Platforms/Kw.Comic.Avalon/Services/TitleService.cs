using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using GalaSoft.MvvmLight;
using Kw.Comic.Avalon.Views;
using Kw.Comic.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Avalon.Services
{
    internal class TitleService : ObservableObject
    {
        public const double FontSizeFactor = 0.45d;
        public TitleService()
        {
            GoBackButton = new Button
            {
                Background = null,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Content = new TextBlock
                {
                    Classes = new Classes("segoblock"),
                    Text = "\xE72B"
                }
            };
            GoBackButton.IsVisible = true;
            GoBackButton.Click += GoBackButton_Click;
            var tbx = new TextBlock();
            TitleControl = tbx;
            tbx.Bind(TextBlock.TextProperty,new Binding(nameof(Title)) { Source = this });
            LeftControls = new ObservableCollection<IControl> { GoBackButton };
        }

        private void GoBackButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var navSer = AppEngine.GetRequiredService<MainNavigationService>();
            var innerType = navSer.border.Child?.GetType();
            if (innerType !=null&& !innerType.IsEquivalentTo(typeof(HomePage)))
            {

            if (innerType.IsEquivalentTo(typeof(VisitingView)))
            {
                navSer.Navigate<ComicView>();
            }
            else
            {
                navSer.Navigate<HomePage>();
            }
            }
        }

        private void BackButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var navSer = AppEngine.GetRequiredService<INavigationService>();
            navSer.GoBack();
        }
        private string title;
        private IDisposable binder;
        private IControl titleControl;
        private double adviseFontSize;
        private double offsceneHeight;
        private bool titleVisible=true;
        private MainWindow window;

        public MainWindow Window
        {
            get { return window; }
            private set => Set(ref window, value);
        }

        public bool TitleVisible
        {
            get { return titleVisible; }
            set => Set(ref titleVisible, value);
        }

        public double OffsceneHeight
        {
            get { return offsceneHeight; }
            private set => Set(ref offsceneHeight, value);
        }

        public double AdviseFontSize
        {
            get { return adviseFontSize; }
            private set => Set(ref adviseFontSize, value);
        }

        public IControl TitleControl
        {
            get { return titleControl; }
            set => Set(ref titleControl, value);
        }
        public string Title
        {
            get => title;
            set
            {
                Set(ref title, value);
            }
        }

        public Button GoBackButton { get; }

        public ObservableCollection<IControl> LeftControls { get; }
        public void UnBind()
        {
            binder?.Dispose();
        }
        public void Bind(MainWindow window)
        {
            UnBind();
            Window = window;
            binder= window.BindDecorationMargin(RaiseMargin);
        }
        private void RaiseMargin(Thickness x)
        {
            OffsceneHeight = (x.Bottom + x.Top);
            AdviseFontSize = OffsceneHeight * FontSizeFactor;
        }
    }
}
