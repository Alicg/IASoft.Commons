using System;

namespace IASoft.WPFCommons.Events
{
    public class PasswordRequestData
    {
        public PasswordRequestData(string title, Action<ConfirmationArgs> resultCallback)
        {
            this.Title = title;
            this.ResultCallback = resultCallback;
        }

        public string Title { get; }
        
        public Action<ConfirmationArgs> ResultCallback { get; }
    }
}