using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace SVA.Infrastructure.Extensions
{
    public static class ReactiveExtensions
    {
        /// <summary>
        /// Taken from http://stackoverflow.com/questions/7597773/does-reactive-extensions-support-rolling-buffers/9791385#9791385.
        /// </summary>
        public static IObservable<IList<T>> BufferUntilInactive<T>(this IObservable<T> stream, TimeSpan delay)
        {
            var closes = stream.Throttle(delay);
            return stream.Window(() => closes).SelectMany(window => window.ToList());
        }
    }
}