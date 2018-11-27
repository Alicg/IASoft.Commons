using System;
using IASoft.WPFCommons.Events;

namespace IASoft.PrismCommons.PendingProcess
{
    public class PendingProcess : IDisposable
    {
        private readonly IReactiveEventAggregator eventAggregator;
        private readonly string description;

        private PendingProcess(IReactiveEventAggregator eventAggregator, string description = null)
        {
            this.eventAggregator = eventAggregator;
            this.description = description;
            this.eventAggregator.Publish(new PendingProcessStartEvent {Description = description});
        }

        public static IDisposable StartNew(IReactiveEventAggregator eventAggregator, string description = null)
        {
            return new PendingProcess(eventAggregator, description);
        }

        public void Dispose()
        {
            this.eventAggregator.Publish(new PendingProcessStopEvent {Description = this.description});
        }
    }
}