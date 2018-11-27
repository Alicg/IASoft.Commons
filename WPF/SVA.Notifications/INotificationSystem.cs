using System;

namespace SVA.Notifications
{
    public interface INotificationSystem
    {
        void PushNotification(NotificationMessage notificationMessage);

        void PushException(Exception exc);
    }
}