using IASoft.WPFCommons.Events;

namespace IASoft.MaterialDesignCommons.PopupWindows
{
    public class StringRequestViewModel : ConfirmationViewModel
    {
        public StringRequestViewModel(IReactiveEventAggregator eventAggregator, ConfirmationArgs confirmationArgs) : base(eventAggregator, confirmationArgs)
        {
        }
    }
}