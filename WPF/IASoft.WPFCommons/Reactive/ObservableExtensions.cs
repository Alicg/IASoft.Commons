using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace IASoft.WPFCommons.Reactive
{
    public static class ObservableExtensions
    {
        public static IObservable<TArgs> FromEventHandler<TArgs>(this object sender, Action<EventHandler<TArgs>> subscribe, Action<EventHandler<TArgs>> unsubscribe)
        {
            return Observable.FromEventPattern<EventHandler<TArgs>, TArgs>(handler => handler.Invoke, subscribe, unsubscribe).Select(e => e.EventArgs);
        }

        public static IObservable<EventArgs> FromEventHandler(this object sender, Action<EventHandler> subscribe, Action<EventHandler> unsubscribe)
        {
            return Observable.FromEventPattern<EventHandler, EventArgs>(handler => handler.Invoke, subscribe, unsubscribe).Select(e => e.EventArgs);
        }

        public static IObservable<TArgs> FromEventHandler<TSender, THandler, TArgs>(
            this TSender sender,
            Func<EventHandler<TArgs>, THandler> conversion,
            Action<THandler> subscribe,
            Action<THandler> unsubscribe)
        {
            return Observable.FromEventPattern(conversion, subscribe, unsubscribe).Select(e => e.EventArgs);
        }

        public static IObservable<(TSender sender, TArgs args)> FromEventHandlerWithSender<TSender, THandler, TArgs>(
            this TSender sender,
            Func<EventHandler<TArgs>, THandler> conversion,
            Action<THandler> subscribe,
            Action<THandler> unsubscribe)
        {
            return Observable.FromEventPattern(conversion, subscribe, unsubscribe).Select(e => (sender, e.EventArgs));
        }

        public static IObservable<PropertyChangedEventArgs> ObservableFromPropertyChanged<T>(this T source, params string[] properties)
            where T : INotifyPropertyChanged
        {
            return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                handler => handler.Invoke,
                h => source.PropertyChanged += h,
                h => source.PropertyChanged -= h)
                .Where(e => properties.Contains(e.EventArgs.PropertyName)).Select(e => e.EventArgs);
        }

        /// <summary>
        /// Returns an observable sequence of the source any time the <c>PropertyChanged</c> event is raised.
        /// </summary>
        /// <typeparam name="T">The type of the source object. Type must implement <seealso cref="INotifyPropertyChanged"/>.</typeparam>
        /// <param name="source">The object to observe property changes on.</param>
        /// <param name="propertiesToIgnore">List of properties which changes should be ignored.</param>
        /// <returns>Returns an observable sequence of the value of the args when ever the <c>PropertyChanged</c> event is raised.</returns>
        public static IObservable<PropertyChangedEventArgs> ObservableFromAnyPropertyChanged<T>(this T source, params string[] propertiesToIgnore)
            where T : INotifyPropertyChanged
        {
            return Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                handler => handler.Invoke,
                h => source.PropertyChanged += h,
                h => source.PropertyChanged -= h)
                .Where(e => !propertiesToIgnore.Contains(e.EventArgs.PropertyName)).Select(e => e.EventArgs);
        }
        
        public static IObservable<NotifyCollectionChangedEventArgs> ObservableFromCollectionChanged<T>(this T source)
            where T : INotifyCollectionChanged
        {
            return Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                handler => handler.Invoke,
                h => source.CollectionChanged += h,
                h => source.CollectionChanged -= h)
                .Select(e => e.EventArgs);
        }

    }
}