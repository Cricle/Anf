using Anf.Desktop.Converters;
using Anf.Desktop.Services;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.Themes.Fluent;

namespace Anf.Desktop
{
    public class SettingsControlView : UserControl
    {
        public SettingsControlView()
        {
            DataContext = this;
            TitleService = AppEngine.GetRequiredService<TitleService>();
            ThemeService = AppEngine.GetRequiredService<ThemeService>();
            InitializeComponent();
        }

        internal TitleService TitleService { get; }
        internal ThemeService ThemeService { get; }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
