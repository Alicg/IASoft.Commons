namespace IASoft.WPFCommons.Events
{
    public class PopupCustomButtonsWindowRequestData
    {
        public PopupCustomButtonsWindowRequestData(string caption, IPageViewModel viewModel)
        {
            this.Caption = caption;
            this.ViewModel = viewModel;
        }

        public string Caption { get; private set; }

        public IPageViewModel ViewModel { get; private set; }
    }
}
