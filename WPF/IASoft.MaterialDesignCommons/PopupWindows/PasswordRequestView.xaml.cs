using System.Windows;
using System.Windows.Controls;

namespace IASoft.MaterialDesignCommons.PopupWindows
{
    /// <summary>
    /// Interaction logic for ConfirmationView.xaml
    /// </summary>
    public partial class PasswordRequestView : UserControl
    {
        public PasswordRequestView()
        {
            this.InitializeComponent();
        }

        private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if ((sender as PasswordRequestView)?.DataContext is PasswordRequestViewModel viewModel)
            {
                viewModel.ConfirmationContent.Content = this.PasswordBox.Password;
            }
        }
    }
}
