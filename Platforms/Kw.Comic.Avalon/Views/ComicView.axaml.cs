using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Kw.Comic.Avalon.Views
{
    public class ComicView : UserControl
    {
        public ComicView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
