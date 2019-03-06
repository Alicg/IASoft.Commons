using ReactiveUI.Legacy;

namespace SVA.Infrastructure.Collections
{
    using System;
    using System.ComponentModel;
    using ReactiveUI;

    public interface ICollectionTrackersFactory
    {
        ICollectionTracker<TItem, EventArgs> GetObjectCommitedCollectionTrackerOnDispatcher<TItem>(IReactiveCollection<TItem> reactiveList)
            where TItem : class, IBaseNotificationDomainObject<TItem>;

        ICollectionTracker<TItem, PropertyChangedEventArgs> GetPropertyChangedCollectionTrackerOnThrottle<TItem>(IReactiveCollection<TItem> reactiveList)
            where TItem : class, IBaseNotificationDomainObject<TItem>;
    }
}