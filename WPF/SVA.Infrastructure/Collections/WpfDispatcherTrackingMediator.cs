namespace SVA.Infrastructure.Collections
{
    using System;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;

    public class WpfDispatcherTrackingMediator : ITrackingMediator
    {
        public IObservable<T> Inject<T>(IObservable<T> observable)
        {
            return observable.ObserveOn(DispatcherScheduler.Current);
        }
    }
}