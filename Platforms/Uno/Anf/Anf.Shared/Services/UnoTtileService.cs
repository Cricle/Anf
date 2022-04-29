using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Anf.Services
{
    internal class UnoTtileService: DependencyAppService
    {
        public const double FontSizeFactor = 0.45d;
        public UnoTtileService()
        {
            var tbx = new TextBlock
            {
                MaxWidth = 280,
                TextTrimming = TextTrimming.WordEllipsis
            };
            TitleControl = tbx;
            tbx.SetBinding(TextBlock.TextProperty, new Binding {Path=new PropertyPath(nameof(Title)), Source = this });
            tbx.SetBinding(ToolTipService.ToolTipProperty, new Binding
            {
                Path = new PropertyPath(nameof(Title)),
                Source = this
            });
            LeftControls = new SilentObservableCollection<FrameworkElement>();
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

            LeftControls.AddRange(new FrameworkElement[] { GoBackButton, FavoriteButton, v });
        }


        private TButton CreateIconButton<TButton>(string text)
            where TButton : Button, new()
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
        private FrameworkElement titleControl;
        private double adviseFontSize;
        private double offsceneHeight;
        private bool titleVisible = true;

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

        public FrameworkElement TitleControl
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

        public SilentObservableCollection<FrameworkElement> LeftControls { get; }
        
        private void RaiseMargin(Thickness x)
        {
            OffsceneHeight = (x.Bottom + x.Top);
            AdviseFontSize = OffsceneHeight * FontSizeFactor;
        }
    }
}
