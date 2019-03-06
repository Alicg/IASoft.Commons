using System.Collections.Generic;
using ReactiveUI.Legacy;

namespace SVA.Infrastructure.Collections
{
    using System;
    using ReactiveUI;

    public abstract class CollectionTrackerBase<TItem, TArgs> : ICollectionTracker<TItem, TArgs> where TArgs : EventArgs where TItem : class, IBaseNotificationDomainObject<TItem>
    {
        private readonly IReactiveCollection<TItem> observableCollection;
        private readonly ITrackingMediator trackingMediator;
        private readonly IList<IDisposable> subscriptions = new List<IDisposable>();

        protected CollectionTrackerBase(IReactiveCollection<TItem> observableCollection, ITrackingMediator trackingMediator = null)
        {
            this.observableCollection = observableCollection;
            this.trackingMediator = trackingMediator;
            this.InitTracker();
        }

        public event EventHandler<CollectionItemChangedEventArgs<TItem, TArgs>> ItemChanged;

        protected abstract IObservable<TArgs> GetObservable(TItem collectionItem);

        private void InitTracker()
        {
            this.SubscribeToAllItems();
            this.observableCollection.ItemsAdded.Subscribe(this.SubscribeToItemEvent);
            this.observableCollection.ShouldReset.Subscribe(resetUnit => this.SubscribeToAllItems());
        }

        private void SubscribeToAllItems()
        {
            lock (this.subscriptions)
            {
                foreach (var subscription in this.subscriptions)
                {
                    subscription.Dispose();
                }
                this.subscriptions.Clear();
            }

            foreach (var item in this.observableCollection)
            {
                this.SubscribeToItemEvent(item);
            }
        }

        private void SubscribeToItemEvent(TItem item)
        {
            lock (this.subscriptions)
            {
                if (this.trackingMediator == null)
                {
                    this.subscriptions.Add(this.GetObservable(item).Subscribe(v => this.OnItemChanged(item, v)));
                }
                else
                {
                    this.subscriptions.Add(this.trackingMediator.Inject(this.GetObservable(item)).Subscribe(v => this.OnItemChanged(item, v)));
                }
            }
        }

        protected virtual void OnItemChanged(TItem item, TArgs e)
        {
            this.ItemChanged?.Invoke(this, new CollectionItemChangedEventArgs<TItem, TArgs>(item, e));
        }
    }
}