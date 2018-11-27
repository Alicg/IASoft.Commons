using System;

namespace IASoft.WPFCommons.Events
{
    public class PopupConfirmationWindowRequestData
    {
        public PopupConfirmationWindowRequestData(string caption, object viewModel) : this(caption, viewModel, c => { })
        {
        }

        public PopupConfirmationWindowRequestData(string caption, object viewModel, Action<ConfirmationArgs> callback)
        {
            this.Caption = caption;
            this.ViewModel = viewModel;
            this.Callback = callback;
        }

        public string Caption { get; private set; }

        public object ViewModel { get; private set; }

        public Action<ConfirmationArgs> Callback { get; private set; }
    }
}
