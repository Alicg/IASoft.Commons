namespace SVA.Infrastructure.Collections
{
    public class CollectionItemChangedEventArgs<TItem, TArgs>
    {
        public CollectionItemChangedEventArgs(TItem collectionItem, TArgs eventArgs)
        {
            this.CollectionItem = collectionItem;
            this.EventArgs = eventArgs;
        }

        public TItem CollectionItem { get; private set; }

        public TArgs EventArgs { get; private set; }
    }
}
