using System;

namespace IASoft.WPFCommons.Events
{
    public class DefaultConfirmationRequestData
    {
        public DefaultConfirmationRequestData(string caption, string message, Action<ConfirmationArgs> confirmarionCallback)
        {
            this.Caption = caption;
            this.Message = message;
            this.ConfirmarionCallback = confirmarionCallback;
        }

        public string Caption { get; private set; }

        public string Message { get; private set; }

        public Action<ConfirmationArgs> ConfirmarionCallback { get; private set; }
    }
}
