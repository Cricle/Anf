using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Kw.Comic.Avalon.Views;

namespace Kw.Comic.Avalon
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            Content = new HomePage();
        }
    }
}
