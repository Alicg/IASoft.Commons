namespace IASoft.WPFCommons.Events
{
    public class NotificationRequestData
    {
        public NotificationRequestData(string caption, string message)
        {
            this.Caption = caption;
            this.Message = message;
        }

        public string Caption { get; private set; }

        public string Message { get; private set; }
    }
}
