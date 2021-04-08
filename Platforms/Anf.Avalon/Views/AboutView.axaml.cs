using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Anf.Avalon.Views
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
