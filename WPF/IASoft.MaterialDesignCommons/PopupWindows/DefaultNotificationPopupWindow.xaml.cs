using System.Windows;
using Prism.Interactivity.InteractionRequest;

namespace IASoft.MaterialDesignCommons.PopupWindows
{
    /// <summary>
    /// Interaction logic for DefaultConfirmationPopupWindow.xaml
    /// </summary>
    public partial class DefaultNotificationPopupWindow
    {
        public DefaultNotificationPopupWindow()
        {
            this.InitializeComponent();
        }
        
        public INotification Notification
        {
            get
            {
                return this.DataContext as INotification;
            }
            set
            {
                this.DataContext = value;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
