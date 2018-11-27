using IASoft.PrismCommons;
using IASoft.WPFCommons.Events;
using Prism.Commands;

namespace IASoft.MaterialDesignCommons.PopupWindows
{
    public class ConfirmationViewModel : BasePrismViewModel
    {
        public ConfirmationViewModel(IReactiveEventAggregator eventAggregator, ConfirmationArgs confirmationContent) : base(eventAggregator)
        {
            this.ConfirmationContent = confirmationContent;
            this.OkCommand = new DelegateCommand(this.Ok);
            this.CancelCommand = new DelegateCommand(this.Cancel);
        }

        public ConfirmationArgs ConfirmationContent { get; set; }

        public DelegateCommand OkCommand { get; }

        public DelegateCommand CancelCommand { get; }

        private void Cancel()
        {
            this.ConfirmationContent.IsConfirmed = false;
            this.Close();
        }

        private void Ok()
        {
            this.ConfirmationContent.IsConfirmed = true;
            this.Close();
        }
    }
}