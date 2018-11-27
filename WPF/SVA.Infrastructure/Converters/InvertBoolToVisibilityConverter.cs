namespace SVA.Infrastructure.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class InvertBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolValue = value as bool?;
            if (boolValue.HasValue && boolValue.Value)
            {
                return false;
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}