namespace SVA.Notifications
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    public class MessageTypeToBackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var messageType = (NotificationMessageType)value;
            switch (messageType)
            {
                case NotificationMessageType.Info:
                    return Brushes.Blue;
                case NotificationMessageType.Warning:
                    return Brushes.Orange;
                case NotificationMessageType.Error:
                    return Brushes.Red;
                case NotificationMessageType.Fatal:
                    return Brushes.DarkRed;
                case NotificationMessageType.GeneralError:
                    return Brushes.Red;
            }
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
