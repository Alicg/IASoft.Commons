namespace SVA.Infrastructure.Collections
{
    using System;
    using System.ComponentModel;
    using System.Reactive.Concurrency;
    using ReactiveUI;

    public class CollectionTrackersFactory : ICollectionTrackersFactory
    {
        public ICollectionTracker<TItem, EventArgs> GetObjectCommitedCollectionTrackerOnDispatcher<TItem>(IReactiveCollection<TItem> reactiveList)
            where TItem : class, IBaseNotificationDomainObject<TItem>
        {
            return new ObjectCommitedCollectionTracker<TItem>(reactiveList, new WpfDispatcherTrackingMediator());
        }

        public ICollectionTracker<TItem, PropertyChangedEventArgs> GetPropertyChangedCollectionTrackerOnThrottle<TItem>(IReactiveCollection<TItem> reactiveList)
            where TItem : class, IBaseNotificationDomainObject<TItem>
        {
            return new PropertyChangedCollectionTracker<TItem>(reactiveList, new ThrottleTrackingMediator(ThreadPoolScheduler.Instance));
        }
    }
}
