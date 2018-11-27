namespace SVA.Notifications
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    public class MessageTypeToForegroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var messageType = (NotificationMessageType)value;
            switch (messageType)
            {
                case NotificationMessageType.Info:
                    return Brushes.White;
                case NotificationMessageType.Warning:
                    return Brushes.White;
                case NotificationMessageType.Error:
                    return Brushes.White;
                case NotificationMessageType.Fatal:
                    return Brushes.White;
                case NotificationMessageType.GeneralError:
                    return Brushes.White;
            }
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
