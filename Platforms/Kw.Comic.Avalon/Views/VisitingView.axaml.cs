using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Kw.Comic.Avalon.Views
{
    public class VisitingView : UserControl
    {
        public VisitingView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
