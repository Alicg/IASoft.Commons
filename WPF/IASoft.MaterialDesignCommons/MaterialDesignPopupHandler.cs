using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using IASoft.MaterialDesignCommons.PopupWindows;
using IASoft.PrismCommons;
using IASoft.WPFCommons.Events;
using IASoft.WPFCommons.Reactive;
using MaterialDesignThemes.Wpf;

namespace IASoft.MaterialDesignCommons
{
    public class MaterialDesignPopupHandler
    {
        private readonly IReactiveEventAggregator eventAggregator;

        public MaterialDesignPopupHandler(IReactiveEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            eventAggregator.GetEvent<DefaultConfirmationRequestData>().ObserveOn(DispatcherScheduler.Current).Subscribe(this.ShowDefaultConfirmationPopup);
            eventAggregator.GetEvent<StringRequestData>().ObserveOn(DispatcherScheduler.Current).Subscribe(this.ShowDefaultRequestStringPopup);
            eventAggregator.GetEvent<PasswordRequestData>().ObserveOn(DispatcherScheduler.Current).Subscribe(this.ShowDefaultRequestPasswordPopup);
            eventAggregator.GetEvent<PopupConfirmationWindowRequestData>().ObserveOn(DispatcherScheduler.Current).Subscribe(this.ShowCustomPopupWindow);
            eventAggregator.GetEvent<PopupCustomButtonsWindowRequestData>().ObserveOn(DispatcherScheduler.Current).Subscribe(this.ShowPopupWindowWithCustomButtons);
        }

        private async void ShowDefaultConfirmationPopup(DefaultConfirmationRequestData confirmationRequestData)
        {
            var confirmationArgs = new ConfirmationArgs { Content = confirmationRequestData.Message, Title = confirmationRequestData.Caption };
            var confirmationViewModel = new ConfirmationViewModel(this.eventAggregator, confirmationArgs);
            var closeDialogClosure = new OpenedClosedDialogClosure(confirmationRequestData.ConfirmarionCallback, 0.1);
            await DialogHost.Show(confirmationViewModel, "RootDialog", closeDialogClosure.OpenHandle, closeDialogClosure.CloseHandle);
        }

        private async void ShowDefaultRequestStringPopup(StringRequestData stringRequestData)
        {
            var confirmationArgs = new ConfirmationArgs { Title = stringRequestData.Title };
            var stringRequestViewModel = new StringRequestViewModel(this.eventAggregator, confirmationArgs);
            var closeDialogClosure = new OpenedClosedDialogClosure(stringRequestData.ResultCallback, 0.1);
            await DialogHost.Show(stringRequestViewModel, "RootDialog", closeDialogClosure.OpenHandle, closeDialogClosure.CloseHandle);
        }

        private async void ShowDefaultRequestPasswordPopup(PasswordRequestData passwordRequestData)
        {
            var confirmationArgs = new ConfirmationArgs { Title = passwordRequestData.Title };
            var passwordRequestViewModel = new PasswordRequestViewModel(this.eventAggregator, confirmationArgs);
            var closeDialogClosure = new OpenedClosedDialogClosure(passwordRequestData.ResultCallback, 0.1);
            await DialogHost.Show(passwordRequestViewModel, "RootDialog", closeDialogClosure.OpenHandle, closeDialogClosure.CloseHandle);
        }

        private async void ShowCustomPopupWindow(PopupConfirmationWindowRequestData popupConfirmationWindowRequestData)
        {
            var confirmationArgs = new ConfirmationArgs {Content = popupConfirmationWindowRequestData.ViewModel, Title = popupConfirmationWindowRequestData.Caption};
            var confirmationViewModel = new ConfirmationViewModel(this.eventAggregator, confirmationArgs);
            var closeDialogClosure = new OpenedClosedDialogClosure(popupConfirmationWindowRequestData.Callback);
            await DialogHost.Show(confirmationViewModel, "RootDialog", closeDialogClosure.OpenHandle, closeDialogClosure.CloseHandle);
        }

        private async void ShowPopupWindowWithCustomButtons(PopupCustomButtonsWindowRequestData popupCustomButtonsWindowRequestData)
        {
            var closeDialogClosure = new OpenedClosedDialogClosure();
            await DialogHost.Show(popupCustomButtonsWindowRequestData.ViewModel, "RootDialog", closeDialogClosure.OpenHandle, closeDialogClosure.CloseHandle);
        }

        private class OpenedClosedDialogClosure
        {
            private readonly Action<ConfirmationArgs> confirmationAction;
            private DateTime? openedTime;
            private readonly double openCloseMinInterval;

            public OpenedClosedDialogClosure(double? openCloseMinInterval = null)
            {
                this.openCloseMinInterval = openCloseMinInterval ?? 1.5;
            }

            public OpenedClosedDialogClosure(Action<ConfirmationArgs> confirmationAction, double? openCloseMinInterval = null) : this(openCloseMinInterval)
            {
                this.confirmationAction = confirmationAction;
            }

            public void OpenHandle(object sender, DialogOpenedEventArgs args)
            {
                this.openedTime = DateTime.Now;
                var closableViewModel = args.Session.Content as BasePrismViewModel;
                closableViewModel?.FromEventHandler(v => closableViewModel.Closed += v, v => closableViewModel.Closed -= v).Subscribe(v => args.Session.Close());
            }

            public async void CloseHandle(object sender, DialogClosingEventArgs args)
            {
                var now = DateTime.Now;
                if (this.openedTime == null || (now - this.openedTime.Value) < TimeSpan.FromSeconds(this.openCloseMinInterval))
                {
                    args.Cancel();
                    return;
                }
                var confirmationViewModel = args.Session.Content as ConfirmationViewModel;
                this.ClearDialogContent(sender as DialogHost);
                if (this.confirmationAction != null && confirmationViewModel != null)
                {
                    this.confirmationAction.Invoke(confirmationViewModel.ConfirmationContent);
                }
            }

            private void ClearDialogContent(DialogHost dialogHost)
            {
                if (dialogHost != null)
                {
                    dialogHost.DialogContent = null;
                }
            }
        }
    }
}