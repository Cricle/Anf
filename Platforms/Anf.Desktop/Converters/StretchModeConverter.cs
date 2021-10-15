using Avalonia.Controls.PanAndZoom;
using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Desktop.Converters
{
    public class StretchModeConverter : IValueConverter
    {
        private static readonly IReadOnlyDictionary<StretchMode, string> nameForModels = new Dictionary<StretchMode, string>
        {
            [StretchMode.Fill] = "填满",
            [StretchMode.None] = "自由",
            [StretchMode.Uniform] = "均匀",
            [StretchMode.UniformToFill] = "满均"
        };
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is StretchMode mode)
            {
                return nameForModels[mode];
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {            
            if (Enum.TryParse<StretchMode>(value?.ToString(), out var model))
            {
                return model;
            }
            return value;
        }
    }
}
