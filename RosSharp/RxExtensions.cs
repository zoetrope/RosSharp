using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;

namespace RosSharp
{
    public static class RxExtensions
    {
        public static IObservable<TSource> RetryWithDelay<TSource>(this IObservable<TSource> source, TimeSpan timeout)
        {
            var exSource = source.Catch<TSource, Exception>(ex => Observable.Empty<TSource>().Delay(timeout));

            return RepeatInfinite(exSource).Concat();
        }

        private static IEnumerable<TSource> RepeatInfinite<TSource>(TSource source)
        {
            while (true)
            {
                yield return source;
            }
        }
    }
}

