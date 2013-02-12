using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;

namespace RosSharp
{
    public static class RxExtensions
    {
        public static IObservable<TSource> RetryWithDelay<TSource>(this IObservable<TSource> source, TimeSpan timeout, IScheduler scheduler)
        {
            IObservable<TSource> exSource = null;
            exSource = source.Catch<TSource, Exception>(ex =>
            {
                Console.WriteLine("catch");
                return Observable.Empty<TSource>().Delay(timeout, scheduler).Concat(exSource);
            });

            return exSource;
        }
        public static IObservable<TSource> RetryWithDelay<TSource>(this IObservable<TSource> source, TimeSpan timeout)
        {
            IObservable<TSource> exSource = null;
            exSource = source.Catch<TSource, Exception>(ex =>
            {
                Console.WriteLine("catch");
                return Observable.Empty<TSource>().Delay(timeout).Concat(exSource);
            });

            return exSource;
        }


        public static IObservable<TSource> RetryWithDelayOld<TSource>(this IObservable<TSource> source, TimeSpan timeout, IScheduler scheduler)
        {
            var exSource = source.Catch<TSource, Exception>(ex => Observable.Empty<TSource>().Delay(timeout,scheduler));

            return RepeatInfinite(exSource).Concat();
        }

        private static IEnumerable<TSource> RepeatInfinite<TSource>(TSource source)
        {
            while (true)
            {
                Console.WriteLine("Repeat");
                yield return source;
            }
        }
    }
}

