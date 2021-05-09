using Anf.Desktop.Converters;
using Anf.Desktop.Services;
using Anf.Desktop.Settings;
using Anf.Platform.Settings;
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
            AnfSettings = AppEngine.GetRequiredService<AnfSettings>();
            ThemeSettings = AnfSettings.Theme;
            ReadingSettings = AnfSettings.Reading;
            PerformaceSettings = AnfSettings.Performace;
            InitializeComponent();
        }

        internal AnfSettings AnfSettings { get; }

        internal ThemeSettings ThemeSettings { get; }
        internal ReadingSettings ReadingSettings { get; }
        internal PerformaceSettings PerformaceSettings { get; }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
