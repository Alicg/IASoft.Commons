using System;
using System.Globalization;
using System.Windows.Data;

namespace SVA.Infrastructure.Converters
{
    public class ValueToEqualsParameterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var doubleValue = value as double?;
            double doubleParameter;
            if (double.TryParse((string)parameter, out doubleParameter) && doubleValue != null)
            {
                return Math.Abs(doubleValue.Value - doubleParameter) < double.Epsilon;
            }
            return value == parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}