using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Anf.Converters
{
    public class SearchProviderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (value is ISearchProvider provider)
            {
                return provider;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            return value;
        }
    }
}
