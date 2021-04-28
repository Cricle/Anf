using Anf.Desktop.Settings;
using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Desktop.Converters
{
    public class StartupTypeStringConverter : IValueConverter
    {
        private static IReadOnlyDictionary<StartupTypes, string> nameForModels = new Dictionary<StartupTypes, string>
        {
            [StartupTypes.None] = "空白",
            [StartupTypes.Proposal] = "推介",
            [StartupTypes.Providers] = "提供者"
        };
        private static IReadOnlyDictionary<string, StartupTypes> modelForNames = nameForModels.ToDictionary(x => x.Value, x => x.Key);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is StartupTypes type)
            {
                return nameForModels[type];
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                return modelForNames[str];
            }
            return null;
        }
    }
    public class StartupTypeBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is StartupTypes type&&parameter is StartupTypes destype)
            {
                return type == destype;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
