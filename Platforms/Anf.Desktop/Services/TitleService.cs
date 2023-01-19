using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Anf.Desktop.Views;
using Anf.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia.Interactivity;
using Avalonia.Controls.Primitives;
using Anf.Desktop.Converters;
using Avalonia.Themes.Fluent;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Anf.Desktop.Services
{
    internal class TitleService : ObservableObject
    {
        public const double FontSizeFactor = 0.45d;
        public TitleService()
        {
            var tbx = new TextBlock
            {
                MaxWidth = 280,
                TextTrimming = TextTrimming.WordEllipsis
            };
            TitleControl = tbx;
            tbx.Bind(TextBlock.TextProperty,new Binding(nameof(Title)) { Source = this });
            tbx.Bind(ToolTip.TipProperty, new Binding(nameof(Title)) { Source = this });
            LeftControls = new SilentObservableCollection<IControl>();
        }

        private void OnFavoriteButtonClick(object sender, RoutedEventArgs e)
        {
            var navSer = AppEngine.GetRequiredService<MainNavigationService>();
            navSer.Navigate<BookshelfView>();
        }

        public void CreateControls()
        {
            GoBackButton = CreateIconButton<Button>("\xE72B");
            GoBackButton.Click += OnGoBackButtonClick;

            FavoriteButton = CreateIconButton<Button>("\xE8F1");
            FavoriteButton.Click += OnFavoriteButtonClick;

            var v = new SettingsControlView();

            LeftControls.AddRange(new IControl[] { GoBackButton, FavoriteButton,v });
        }


        private TButton CreateIconButton<TButton>(string text)
            where TButton:Button,new()
        {
            var btn = new TButton
            {
                Background = null,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Content = new TextBlock
                {
                    Classes = new Classes("segoblock"),
                    Text = text
                }
            };
            btn.IsVisible = true;
            btn.Bind(Button.FontSizeProperty, new Binding(nameof(AdviseFontSize)) { Source = this });
            return btn;
        }
        private void OnGoBackButtonClick(object sender, RoutedEventArgs e)
        {
            var navSer = AppEngine.GetRequiredService<MainNavigationService>();
            navSer.GoBack();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var navSer = AppEngine.GetRequiredService<MainNavigationService>();
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
            private set => SetProperty(ref window, value);
        }

        public bool TitleVisible
        {
            get { return titleVisible; }
            set => SetProperty(ref titleVisible, value);
        }

        public double OffsceneHeight
        {
            get { return offsceneHeight; }
            private set => SetProperty(ref offsceneHeight, value);
        }

        public double AdviseFontSize
        {
            get { return adviseFontSize; }
            private set => SetProperty(ref adviseFontSize, value);
        }

        public IControl TitleControl
        {
            get { return titleControl; }
            set => SetProperty(ref titleControl, value);
        }
        public string Title
        {
            get => title;
            set
            {
                SetProperty(ref title, value);
            }
        }

        public Button GoBackButton { get; private set; }
        public Button FavoriteButton { get; private set; }

        public SilentObservableCollection<IControl> LeftControls { get; }
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
