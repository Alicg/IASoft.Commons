using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace SVA.Infrastructure.Collections
{
    public class GroupHeaderTypesToStringConverter : IValueConverter
    {
        private readonly Dictionary<GroupHeaderTypes, string> groupHeaderNames = new Dictionary<GroupHeaderTypes, string>
        {
            {GroupHeaderTypes.All, "All"},
            {GroupHeaderTypes.Today, "Today"},
            {GroupHeaderTypes.Yesterday, "Yesterday"},
            {GroupHeaderTypes.ThisWeek, "This week"},
            {GroupHeaderTypes.ThisMonth, "This month"},
            {GroupHeaderTypes.LastMonth, "Last month"},
            {GroupHeaderTypes.ThisYear, "This year"},
            {GroupHeaderTypes.LastYear, "Last year"},
            {GroupHeaderTypes.LongAgo, "Long ago"},
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var groupHeaderType = (GroupHeaderTypes)value;
            return this.groupHeaderNames[groupHeaderType];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}