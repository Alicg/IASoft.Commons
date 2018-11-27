using System;
using System.Windows.Controls;
using IASoft.WPFCommons.Events;

namespace SVA.Notifications
{
    /// <summary>
    /// Interaction logic for NotificationView.xaml
    /// </summary>
    public partial class NotificationView : UserControl
    {
        public NotificationView()
        {
            InitializeComponent();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var viewModel = (NotificationViewModel)this.DataContext;
            viewModel.EventAggregator.GetEvent<FullscreenOnRequestEvent>().Subscribe(v => this.ContentExpander.IsExpanded = false);
        }
    }
}
