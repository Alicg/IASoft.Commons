using System;
using System.Globalization;
using System.Windows.Data;

namespace SVA.Infrastructure.Converters
{
    public class MillisecondsToTimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var milliseconds = value as double?;
            if (milliseconds == null)
            {
                return null;
            }
            return double.IsPositiveInfinity(milliseconds.Value) ? "Not set" : TimeSpan.FromMilliseconds(milliseconds.Value).ToString(@"hh\:mm\:ss");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}