using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace IASoft.WPFCommons.Events
{
    /// <summary>
    /// Class was copied from https://github.com/shiftkey/Reactive.EventAggregator. 
    /// </summary>
    public class ReactiveEventAggregator : IReactiveEventAggregator
    {
        private readonly Subject<object> subject = new Subject<object>();

        private bool disposed;

        public IObservable<TEvent> GetEvent<TEvent>()
        {
            return this.subject.OfType<TEvent>().AsObservable();
        }

        public void Publish<TEvent>(TEvent sampleEvent)
        {
            this.subject.OnNext(sampleEvent);
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.subject.Dispose();

            this.disposed = true;
        }
    }
}