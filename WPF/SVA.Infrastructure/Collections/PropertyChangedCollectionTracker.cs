using IASoft.WPFCommons.Reactive;

namespace SVA.Infrastructure.Collections
{
    using System;
    using System.ComponentModel;
    using ReactiveUI;

    public class PropertyChangedCollectionTracker<TItem> : CollectionTrackerBase<TItem, PropertyChangedEventArgs> where TItem : class, IBaseNotificationDomainObject<TItem>
    {
        public PropertyChangedCollectionTracker(IReactiveCollection<TItem> observableCollection, ITrackingMediator trackingMediator = null)
            : base(observableCollection, trackingMediator)
        {
        }

        protected override IObservable<PropertyChangedEventArgs> GetObservable(TItem collectionItem)
        {
            return collectionItem.ObservableFromAnyPropertyChanged(nameof(IBaseNotificationDomainObject<TItem>.ContentCommitted));
        }
    }
}