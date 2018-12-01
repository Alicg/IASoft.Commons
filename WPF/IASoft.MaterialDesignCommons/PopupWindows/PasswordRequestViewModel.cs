using IASoft.WPFCommons.Events;

namespace IASoft.MaterialDesignCommons.PopupWindows
{
    public class PasswordRequestViewModel : ConfirmationViewModel
    {
        public PasswordRequestViewModel(IReactiveEventAggregator eventAggregator, ConfirmationArgs confirmationArgs) : base(eventAggregator, confirmationArgs)
        {
        }
    }
}