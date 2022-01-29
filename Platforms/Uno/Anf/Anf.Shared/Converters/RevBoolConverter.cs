using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Anf.Converters
{
    public class RevBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (value is bool b)
            {
                return !b;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotSupportedException();
        }
    }
}
