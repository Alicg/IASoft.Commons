using SVA.Infrastructure.Collections;

namespace SVA.Infrastructure
{
    using System;
    using System.Reactive.Linq;

    public static class NotificationExtensions
    {
        public static IObservable<ObjectCommitedEventArgs<TSnapshot>> ObservableFromObjectCommited<TSnapshot>(this IBaseNotificationDomainObject<TSnapshot> source)
            where TSnapshot : class, IBaseNotificationDomainObject<TSnapshot>
        {
            return Observable.FromEventPattern<EventHandler<ObjectCommitedEventArgs<TSnapshot>>, ObjectCommitedEventArgs<TSnapshot>>(
                handler => handler.Invoke,
                h => source.ObjectCommited += h,
                h => source.ObjectCommited -= h)
                .Select(e => e.EventArgs);
        }
    }
}
