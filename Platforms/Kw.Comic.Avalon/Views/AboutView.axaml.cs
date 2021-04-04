using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Kw.Comic.Avalon.Views
{
    public class AboutView : UserControl
    {
        public AboutView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
