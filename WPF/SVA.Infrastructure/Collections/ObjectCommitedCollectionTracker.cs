using ReactiveUI.Legacy;

namespace SVA.Infrastructure.Collections
{
    using System;
    using System.Reactive.Linq;
    using ReactiveUI;

    public class ObjectCommitedCollectionTracker<TItem> : CollectionTrackerBase<TItem, EventArgs> where TItem : class, IBaseNotificationDomainObject<TItem>
    {
        public ObjectCommitedCollectionTracker(IReactiveCollection<TItem> observableCollection, ITrackingMediator trackingMediator = null)
            : base(observableCollection, trackingMediator)
        {
        }

        protected override IObservable<EventArgs> GetObservable(TItem collectionItem)
        {
            return OnObjectCommited(collectionItem);
        }

        private static IObservable<ObjectCommitedEventArgs<TItem>> OnObjectCommited(TItem source)
        {
            return Observable.FromEventPattern<EventHandler<ObjectCommitedEventArgs<TItem>>, ObjectCommitedEventArgs<TItem>>(
                handler => handler.Invoke,
                h => source.ObjectCommited += h,
                h => source.ObjectCommited -= h)
                .Select(e => e.EventArgs);
        }
    }
}