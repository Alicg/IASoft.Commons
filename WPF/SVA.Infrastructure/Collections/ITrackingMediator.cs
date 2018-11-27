namespace SVA.Infrastructure.Collections
{
    using System;

    public interface ITrackingMediator
    {
        IObservable<T> Inject<T>(IObservable<T> observable);
    }
}