using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace SVA.Infrastructure.Converters
{
    public class EnumerableAnyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var enumerable = values[0] as IEnumerable;
            if (enumerable != null)
            {
                return enumerable.GetEnumerator().MoveNext();
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}