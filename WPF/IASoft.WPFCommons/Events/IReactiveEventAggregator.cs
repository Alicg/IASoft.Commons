using System;

namespace IASoft.WPFCommons.Events
{
    public interface IReactiveEventAggregator
    {
        IObservable<TEvent> GetEvent<TEvent>();

        void Publish<TEvent>(TEvent sampleEvent);
    }
}
