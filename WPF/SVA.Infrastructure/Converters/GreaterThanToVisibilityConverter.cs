using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SVA.Infrastructure.Converters
{
    public class GreaterThanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var numericValue = System.Convert.ToInt64(value);
            var numericParameter = System.Convert.ToInt64(parameter);
            if (numericValue > numericParameter)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}