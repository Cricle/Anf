using Anf.Desktop.Services;
using Avalonia.Themes.Fluent;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Anf.Desktop.Settings
{
    public class ThemeSettings : ObservableObject
    {
        private static ThemeService ThemeService => AppEngine.GetRequiredService<ThemeService>();
        
        public virtual bool EnableAcrylicBlur
        {
            get => ThemeService.EnableAcrylicBlur;
            set => ThemeService.EnableAcrylicBlur = value;
        }
        public virtual bool EnableDrakTheme
        {
            get => ThemeService.CurrentModel == FluentThemeMode.Dark;
            set
            {
                ThemeService.CurrentModel = value ? FluentThemeMode.Dark : FluentThemeMode.Light;
            }
        }

    }
}
