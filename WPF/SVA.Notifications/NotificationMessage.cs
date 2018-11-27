using System;
using System.Runtime.CompilerServices;

namespace SVA.Notifications
{
    public class NotificationMessage
    {
        private const int ShortMessageTextLength = 35;

        /// <param name="messageTime"/>
        /// <param name="messageType"/>
        /// <param name="messageText"/>
        /// <param name="shortMessageText">Короткий вариант сообщения до 20 символов.</param>
        /// <param name="source">Окно/Класс в котором возникло сообщение.</param>
        public NotificationMessage(DateTime messageTime, NotificationMessageType messageType, string messageText, string shortMessageText, string source)
        {
            this.MessageTime = messageTime;
            this.MessageText = messageText;
            this.MessageType = messageType;
            this.ShortMessageText = shortMessageText;
            this.Source = source;
        }

        /// <param name="messageType"/>
        /// <param name="messageText"/>
        /// <param name="shortMessageText">Короткий вариант сообщения до 20 символов.</param>
        /// <param name="source">Окно/Класс в котором возникло сообщение.</param>
        public NotificationMessage(NotificationMessageType messageType, string messageText, string shortMessageText, string source)
            : this(DateTime.Now, messageType, messageText, shortMessageText, source)
        {
        }

        /// <param name="messageType"/>
        /// <param name="messageText"/>
        /// <param name="source">Окно/Класс в котором возникло сообщение.</param>
        public NotificationMessage(NotificationMessageType messageType, string messageText, [CallerMemberName]string source = null)
            : this(DateTime.Now, messageType, messageText, messageText.Substring(0, Math.Min(messageText.Length, ShortMessageTextLength)), source)
        {
        }

        public DateTime MessageTime { get; private set; }

        public NotificationMessageType MessageType { get; private set; }

        public string MessageText { get; private set; }

        public string AdditionalMessageText { get; set; }

        public string ShortMessageText { get; private set; }

        public string Source { get; private set; }
    }
}