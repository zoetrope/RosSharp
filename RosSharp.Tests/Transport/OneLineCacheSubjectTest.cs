using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RosSharp.Transport;

namespace RosSharp.Tests.Transport
{
    [TestClass]
    public class OneLineCacheSubjectTest : ReactiveTest
    {
        [TestMethod]
        public void OneLineTest()
        {
            var scheduler = new TestScheduler();

            var observer = scheduler.CreateObserver<int>();

            var sub = new OneLineCacheSubject<int>();

            sub.OnNext(1);

            var d1 = sub.Subscribe(observer);
            sub.OnNext(2);

            d1.Dispose();

            sub.OnNext(3);
            sub.OnNext(4);
            sub.OnNext(5);

            var d2 = sub.Subscribe(observer);

            d2.Dispose();

            var d3 = sub.Subscribe(observer);
            sub.OnNext(6);

            observer.Messages.Select(x => x.Value.Value).
                Is(new List<int>() {1, 2, 3, 4, 5, 6});
        }
    }
}
