namespace IASoft.WPFCommons.Events
{
    public class ConfirmationArgs
    {
        public string Title { get; set; } 

        public object Content { get; set; }

        public bool IsConfirmed { get; set; }
    }
}