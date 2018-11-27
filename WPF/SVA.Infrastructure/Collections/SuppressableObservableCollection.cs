using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace SVA.Infrastructure.Collections
{
    public class SuppressableObservableCollection<T> : ObservableCollection<T>
    {
        private bool suppressNotification;

        /// <summary>
        /// Если true, то коллекция не генерирует события об измененении.
        /// </summary>
        public bool SuppressNotification
        {
            get
            {
                return this.suppressNotification;
            }
            set
            {
                if (value == this.suppressNotification)
                {
                    return;
                }
                this.suppressNotification = value;
                if (!value)
                {
                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
            }
        }

        public void ReinitSuppressable(IEnumerable<T> items)
        {
            this.SuppressNotification = true;
            this.Clear();
            this.AddRange(items);
            this.SuppressNotification = false;
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!this.SuppressNotification)
                base.OnCollectionChanged(e);
        }
    }
}
