using System;
using System.Globalization;
using System.Windows.Data;

namespace SVA.Infrastructure.Converters
{
    public class DivideOnConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var numericValue = System.Convert.ToDouble(value);
            var numericParameter = System.Convert.ToDouble(parameter);
            return numericValue / numericParameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}