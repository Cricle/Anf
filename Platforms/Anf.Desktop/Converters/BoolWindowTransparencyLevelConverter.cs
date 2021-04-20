using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Themes.Fluent;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Desktop.Converters
{
    public class BoolWindowTransparencyLevelConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is WindowTransparencyLevel level)
            {
                return level == WindowTransparencyLevel.AcrylicBlur;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool?)(value) == true ? WindowTransparencyLevel.AcrylicBlur : WindowTransparencyLevel.None;

        }
    }
    public class BoolFluentThemeModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FluentThemeMode level)
            {
                return level == FluentThemeMode.Dark;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool?)(value) == true ? FluentThemeMode.Dark : FluentThemeMode.Light;

        }
    }
}
