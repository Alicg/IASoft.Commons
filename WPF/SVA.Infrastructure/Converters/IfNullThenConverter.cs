using System;
using System.Globalization;
using System.Windows.Data;

namespace SVA.Infrastructure.Converters
{
    public class IfNullThenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ?? parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}