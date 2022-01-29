using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Anf.Converters
{
    public class AddNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (value is int vi)
            {
                return vi+1;
            }
            else if (value is long vl)
            {
                return vl+1;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotSupportedException();
        }
    }
}
