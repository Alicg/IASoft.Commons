namespace SVA.Infrastructure.Collections
{
    using System;
    using System.Reactive.Concurrency;

    public class ThrottleTrackingMediator : ITrackingMediator
    {
        private const double DefaultIgnoreConsequentCallsInterval = 200;
        private readonly double throttleTime;
        private readonly IScheduler scheduler;

        public ThrottleTrackingMediator() : this(DefaultIgnoreConsequentCallsInterval)
        {
        }

        public ThrottleTrackingMediator(double throttleTime) : this(throttleTime, Scheduler.Default)
        {
        }

        public ThrottleTrackingMediator(IScheduler scheduler) : this(DefaultIgnoreConsequentCallsInterval, scheduler)
        {
        }

        public ThrottleTrackingMediator(double throttleTime, IScheduler scheduler)
        {
            this.throttleTime = throttleTime;
            this.scheduler = scheduler;
        }

        public IObservable<T> Inject<T>(IObservable<T> observable)
        {
            return observable;
        }
    }
}