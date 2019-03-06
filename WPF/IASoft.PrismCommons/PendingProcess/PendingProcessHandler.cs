using System;
using System.Reactive.Linq;
using System.Windows.Threading;
using IASoft.WPFCommons.Events;
using Prism.Mvvm;
using ReactiveUI;
using ReactiveUI.Legacy;

namespace IASoft.PrismCommons.PendingProcess
{
    public class PendingProcessHandler : BindableBase
    {
        private int pendingProcessRequestsCount;


        public PendingProcessHandler(IReactiveEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<PendingProcessStartEvent>().ObserveOn(Dispatcher.CurrentDispatcher, DispatcherPriority.Render).Subscribe(v =>
            {
                this.pendingProcessRequestsCount++;
                this.PendingProcessDescriptions.Add(v.Description);
                this.OnPropertyChanged(() => this.PendingProcessIsActive);
            });
            eventAggregator.GetEvent<PendingProcessStopEvent>().ObserveOn(Dispatcher.CurrentDispatcher, DispatcherPriority.Render).Subscribe(v =>
            {
                this.pendingProcessRequestsCount--;
                this.PendingProcessDescriptions.Remove(v.Description);
                this.OnPropertyChanged(() => this.PendingProcessIsActive);
            });
        }

        public IReactiveList<string> PendingProcessDescriptions { get; } = new ReactiveList<string>();

        public bool PendingProcessIsActive => this.pendingProcessRequestsCount > 0;
    }
}