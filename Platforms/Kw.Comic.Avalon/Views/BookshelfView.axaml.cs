using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Kw.Comic.Avalon.Views
{
    public class BookshelfView : UserControl
    {
        public BookshelfView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
