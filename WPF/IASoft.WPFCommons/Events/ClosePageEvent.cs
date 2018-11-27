namespace IASoft.WPFCommons.Events
{
    public class ClosePageEvent
    {
        public ClosePageEvent(IPageViewModel pageViewModel)
        {
            this.ViewModel = pageViewModel;
        }

        public IPageViewModel ViewModel { get; }
    }
}