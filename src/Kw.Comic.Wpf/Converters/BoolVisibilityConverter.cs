using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Kw.Comic.Wpf.Converters
{
    public class BoolRevConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
        {
            if (value is bool b)
            {
                return !b;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
        {
            if (value is bool b)
            {
                return !b;
            }
            return null;
        }
    }
    public class BoolVisibilityConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
        {
            if (value is bool b)
            {
                if (parameter?.ToString() == "rev")
                {
                    b = !b;
                }
                return b ? Visibility.Visible : Visibility.Collapsed;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
        {
            if (value is Visibility visibility)
            {
                var r = visibility == Visibility.Visible;
                if (parameter?.ToString()=="rev")
                {
                    r = !r;
                }
                return r;
            }
            return null;
        }
    }
}
