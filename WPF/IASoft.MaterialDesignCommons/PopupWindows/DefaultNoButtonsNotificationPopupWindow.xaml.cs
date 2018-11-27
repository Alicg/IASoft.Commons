using IASoft.WPFCommons;

namespace IASoft.MaterialDesignCommons.PopupWindows
{
    /// <summary>
    /// Interaction logic for DefaultConfirmationPopupWindow.xaml
    /// </summary>
    public partial class DefaultNoButtonsNotificationPopupWindow
    {
        public DefaultNoButtonsNotificationPopupWindow()
        {
            this.InitializeComponent();
        }
        
        public IPageViewModel PageViewModel
        {
            get
            {
                return this.DataContext as IPageViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }
    }
}
