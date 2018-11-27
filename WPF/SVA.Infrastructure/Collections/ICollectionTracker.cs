namespace SVA.Infrastructure.Collections
{
    using System;

    public interface ICollectionTracker<TItem, TArgs> where TArgs : EventArgs
    {
        event EventHandler<CollectionItemChangedEventArgs<TItem, TArgs>> ItemChanged;
    }
}