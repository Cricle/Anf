using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Anf.Desktop.Views
{
    public partial class SettingView : UserControl
    {
        public SettingView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
