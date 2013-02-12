using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RosSharp.UnitTests
{
    [TestClass]
    public class RxExtensionsTest : ReactiveTest
    {
        [TestMethod]
        public void Retry_AllOnNext()
        {
            var scheduler = new TestScheduler();

            var source = scheduler.CreateHotObservable(OnNext(100, 1), OnNext(200, 2), OnNext(300, 3));
            var observer = scheduler.CreateObserver<int>();

            source.Retry()
                .Subscribe(observer);

            scheduler.AdvanceTo(300);

            observer.Messages.Is(new[] {OnNext(100, 1), OnNext(200, 2), OnNext(300, 3)});
        }

        [TestMethod]
        public void Retry_ColdError()
        {
            var scheduler = new TestScheduler();

            // ColdかHotかで挙動が違う！！
            var source = scheduler.CreateColdObservable(OnNext(100, 1), OnError<int>(200, new Exception()), OnNext(300, 3));
            var observer = scheduler.CreateObserver<int>();

            source.Retry()
                .Subscribe(observer);

            scheduler.AdvanceTo(300);

            // 同じ値が2回来る！
            observer.Messages.Is(new[] { OnNext(100, 1), OnNext(300, 1) });
        }
        [TestMethod]
        public void Retry_HotError()
        {
            var scheduler = new TestScheduler();

            // ColdかHotかで挙動が違う！！
            var source = scheduler.CreateHotObservable(OnNext(100, 1), OnError<int>(200, new Exception()), OnNext(300, 3));
            var observer = scheduler.CreateObserver<int>();

            source.Retry()
                .Subscribe(observer);

            scheduler.AdvanceTo(300);

            observer.Messages.Is(new[] { OnNext(100, 1), OnNext(300, 3) });
        }


        [TestMethod]
        public void RetryWithDelay_AllOnNext()
        {
            var scheduler = new TestScheduler();

            var source = scheduler.CreateHotObservable(OnNext(100, 1), OnNext(200, 2), OnNext(300, 3));
            var observer = scheduler.CreateObserver<int>();

            source.RetryWithDelay(TimeSpan.FromTicks(50), scheduler)
                .Subscribe(observer);

            scheduler.AdvanceTo(300);

            observer.Messages.Is(new[] { OnNext(100, 1), OnNext(200, 2), OnNext(300, 3) });
        }

        [TestMethod]
        public void RetryWithDelay_ColdError()
        {
            var scheduler = new TestScheduler();

            // ColdかHotかで挙動が違う！！
            var source = scheduler.CreateColdObservable(OnNext(100, 1), OnError<int>(200, new Exception()), OnNext(300, 3));
            var observer = scheduler.CreateObserver<int>();

            source.RetryWithDelay(TimeSpan.FromTicks(50), scheduler)
                .Subscribe(observer);

            scheduler.AdvanceTo(300);

            // 2つめがこない？
            observer.Messages.Is(new[] { OnNext(100, 1)});
        }
        [TestMethod]
        public void RetryWithDelay_HotError()
        {
            var scheduler = new TestScheduler();

            // ColdかHotかで挙動が違う！！
            var source = scheduler.CreateHotObservable(OnNext(100, 1), OnError<int>(200, new Exception()), OnNext(300, 3));
            var observer = scheduler.CreateObserver<int>();

            source.RetryWithDelay(TimeSpan.FromTicks(50),scheduler)
                //.SubscribeOn(scheduler)
                .Subscribe(observer);

            scheduler.AdvanceTo(500);

            observer.Messages.Is(new[] { OnNext(100, 1), OnNext(300, 3) });
        }

        [TestMethod]
        public void RetryWithDelay1_AllOnNext()
        {
            var scheduler = new TestScheduler();

            var source = scheduler.CreateHotObservable(OnNext(100, 1), OnNext(200, 2), OnNext(300, 3));
            var observer = scheduler.CreateObserver<int>();

            source.RetryWithDelayOld(TimeSpan.FromTicks(50), scheduler)
                .Subscribe(observer);

            scheduler.AdvanceTo(300);

            observer.Messages.Is(new[] { OnNext(100, 1), OnNext(200, 2), OnNext(300, 3) });
        }

        [TestMethod]
        public void RetryWithDelay1_ColdError()
        {
            var scheduler = new TestScheduler();

            // ColdかHotかで挙動が違う！！
            var source = scheduler.CreateColdObservable(OnNext(100, 1), OnError<int>(200, new Exception()), OnNext(300, 3));
            var observer = scheduler.CreateObserver<int>();

            source.RetryWithDelayOld(TimeSpan.FromTicks(50), scheduler)
                .Subscribe(observer);

            scheduler.AdvanceTo(300);

            // 2つめがこない？
            observer.Messages.Is(new[] { OnNext(100, 1) });
        }
        [TestMethod]
        public void RetryWithDelay1_HotError()
        {
            var scheduler = new TestScheduler();

            // ColdかHotかで挙動が違う！！
            var source = scheduler.CreateHotObservable(OnNext(100, 1), OnError<int>(200, new Exception()), OnNext(300, 3));
            var observer = scheduler.CreateObserver<int>();

            source.RetryWithDelayOld(TimeSpan.FromTicks(50), scheduler)
                //.SubscribeOn(scheduler)
                .Subscribe(observer);

            scheduler.AdvanceTo(300);

            observer.Messages.Is(new[] { OnNext(100, 1), OnNext(300, 3) });
        }
        [TestMethod]
        public void Rx_Retry()
        {
            Observable.Defer<int>(
                () => Task<int>.Factory.StartNew(() =>
                {
                    Thread.Sleep(1000);
                    Console.WriteLine("Execute! {0}", DateTime.Now);
                    throw new Exception("hoge");
                }).ToObservable())
                .Retry(5)
                .Timeout(TimeSpan.FromSeconds(3))
                .Subscribe(
                    x => { Console.WriteLine("data = {0}", x); },
                    ex => { Console.WriteLine("error = {0}", ex.Message); },
                    () => Console.WriteLine("Completed")
                );

            Thread.Sleep(6000);
        }

        [TestMethod]
        public void Rx_RetryAndDelay()
        {
            Observable.Defer<int>(
                () => Task<int>.Factory.StartNew(() =>
                {
                    Console.WriteLine("Execute! {0}", DateTime.Now);
                    throw new Exception("hoge");
                }).ToObservable())
                .RetryWithDelayOld(TimeSpan.FromSeconds(1),Scheduler.ThreadPool)
                .Timeout(TimeSpan.FromSeconds(5))
                .Subscribe(
                    x => { Console.WriteLine("data = {0}", x); },
                    ex => { Console.WriteLine("error = {0}", ex.Message); },
                    () => Console.WriteLine("Completed")
                );

            Thread.Sleep(6000);
        }

    }
}
