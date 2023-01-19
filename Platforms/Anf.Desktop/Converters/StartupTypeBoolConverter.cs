using Anf.Desktop.Settings;
using Anf.Platform.Settings;
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
        public static readonly IReadOnlyCollection<StartupTypes> IncludeTypes = Enum.GetValues(typeof(StartupTypes)).OfType<StartupTypes>().ToArray();
        public static readonly IReadOnlyDictionary<StartupTypes, string> NameForModels = new Dictionary<StartupTypes, string>
        {
            [StartupTypes.None] = "空白",
            [StartupTypes.Proposal] = "推介",
            [StartupTypes.Providers] = "提供者"
        };
        private static readonly IReadOnlyDictionary<string, StartupTypes> modelForNames = NameForModels.ToDictionary(x => x.Value, x => x.Key);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is StartupTypes type)
            {
                return NameForModels[type];
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
