using System;
using System.Windows;
using System.Windows.Input;
using Prism.Interactivity.InteractionRequest;

namespace IASoft.MaterialDesignCommons.PopupWindows
{
    /// <summary>
    /// Interaction logic for DefaultConfirmationPopupWindow.xaml
    /// </summary>
    public partial class DefaultRequestStringPopupWindow
    {
        public DefaultRequestStringPopupWindow()
        {
            this.InitializeComponent();
        }
        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);

            this.InvalidateMeasure();
        }

        public IConfirmation Confirmation
        {
            get
            {
                return this.DataContext as IConfirmation;
            }
            set
            {
                this.DataContext = value;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Confirmation.Confirmed = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Confirmation.Confirmed = false;
            this.Close();
        }

        private void DefaultRequestStringPopupWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.OkButton_Click(sender, null);
            }
            if (e.Key == Key.Escape)
            {
                this.CancelButton_Click(sender, null);
            }
        }
    }
}
