using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RosSharp.Master;
using RosSharp.Transport;
using RosSharp.std_msgs;

namespace RosSharp.IntegrationTests
{
    [TestClass]
    public class PublisherAndSubscriberTest : ReactiveTest
    {
        private MasterServer _masterServer;
        private static TimeSpan TestTimeout = TimeSpan.FromSeconds(3);

        [TestInitialize]
        public void Initialize()
        {
            Ros.MasterUri = new Uri("http://localhost:11311/");
            Ros.HostName = "localhost";
            Ros.TopicTimeout = 1000;
            Ros.XmlRpcTimeout = 1000;

            _masterServer = new MasterServer(11311);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _masterServer.Dispose();
            Ros.Dispose();
        }

        [TestMethod]
        public void LargeData()
        {
            var observer = new OneLineCacheSubject<std_msgs.ByteMultiArray>();

            var node = Ros.InitNodeAsync("test").Result;

            var publisher = node.PublisherAsync<std_msgs.ByteMultiArray>("test_topic").Result;
            var subscriber = node.SubscriberAsync<std_msgs.ByteMultiArray>("test_topic").Result;

            publisher.WaitForConnection(TestTimeout);
            subscriber.WaitForConnection(TestTimeout);

            subscriber.Subscribe(observer);

            publisher.OnNext(new ByteMultiArray() { data = Enumerable.Range(0, 5000).Select(x => (byte)(x % 256)).ToList() });

            var data = observer.Timeout(TestTimeout).Wait();

            data.data.Count.Is(5000);

            subscriber.Dispose();
            publisher.Dispose();

            node.Dispose();
        }
        [TestMethod]
        public void PublishAndSubscribe()
        {
            var scheduler = new TestScheduler();

            var observer = new OneLineCacheSubject<std_msgs.String>();

            var obs = scheduler.CreateHotObservable(
                OnNext(10, new std_msgs.String() {data = "abc"}),
                OnNext(20, new std_msgs.String() {data = "defg"}),
                OnNext(30, new std_msgs.String() {data = "hijklmn"})
                );

            var node = Ros.InitNodeAsync("test").Result;

            var publisher = node.PublisherAsync<std_msgs.String>("test_topic").Result;
            var subscriber = node.SubscriberAsync<std_msgs.String>("test_topic").Result;

            publisher.WaitForConnection(TestTimeout);
            subscriber.WaitForConnection(TestTimeout);

            subscriber.Subscribe(observer);
            obs.Subscribe(publisher);

            scheduler.AdvanceBy(10);
            observer.Timeout(TestTimeout).Wait().data.Is("abc");

            scheduler.AdvanceBy(10);
            observer.Timeout(TestTimeout).Wait().data.Is("defg");
            
            scheduler.AdvanceBy(10);
            observer.Timeout(TestTimeout).Wait().data.Is("hijklmn");
            
            

            subscriber.Dispose();
            publisher.Dispose();

            node.Dispose();
        }

        [TestMethod]
        public void SubscribeAndPublish()
        {
            var scheduler = new TestScheduler();

            var observer = new OneLineCacheSubject<std_msgs.String>();

            var obs = scheduler.CreateHotObservable(
                OnNext(10, new std_msgs.String() { data = "abc" }),
                OnNext(20, new std_msgs.String() { data = "defg" }),
                OnNext(30, new std_msgs.String() { data = "hijklmn" })
                );

            var node = Ros.InitNodeAsync("test").Result;

            var subscriber = node.SubscriberAsync<std_msgs.String>("test_topic").Result;
            var publisher = node.PublisherAsync<std_msgs.String>("test_topic").Result;

            publisher.WaitForConnection(TestTimeout);
            subscriber.WaitForConnection(TestTimeout);

            subscriber.Subscribe(observer);
            obs.Subscribe(publisher);

            scheduler.AdvanceBy(10);
            observer.Timeout(TestTimeout).Wait().data.Is("abc");

            scheduler.AdvanceBy(10);
            observer.Timeout(TestTimeout).Wait().data.Is("defg");

            scheduler.AdvanceBy(10);
            observer.Timeout(TestTimeout).Wait().data.Is("hijklmn");


            subscriber.Dispose();
            publisher.Dispose();

            node.Dispose();
        }

        [TestMethod]
        public void MultipleSubscriber()
        {
            var scheduler = new TestScheduler();

            var observer1 = new OneLineCacheSubject<std_msgs.String>();
            var observer2 = new OneLineCacheSubject<std_msgs.String>();
            var observer3 = new OneLineCacheSubject<std_msgs.String>();

            var obs = scheduler.CreateHotObservable(
                OnNext(10, new std_msgs.String() { data = "abc" }),
                OnNext(20, new std_msgs.String() { data = "defg" }),
                OnNext(30, new std_msgs.String() { data = "hijklmn" })
                );

            var node1 = Ros.InitNodeAsync("test1").Result;
            var node2 = Ros.InitNodeAsync("test2").Result;
            var node3 = Ros.InitNodeAsync("test3").Result;

            var subscriber1 = node1.SubscriberAsync<std_msgs.String>("test_topic").Result;
            var subscriber2 = node2.SubscriberAsync<std_msgs.String>("test_topic").Result;
            var publisher = node1.PublisherAsync<std_msgs.String>("test_topic").Result;
            var subscriber3 = node3.SubscriberAsync<std_msgs.String>("test_topic").Result;

            publisher.WaitForConnection(TestTimeout);
            subscriber1.WaitForConnection(TestTimeout);
            subscriber2.WaitForConnection(TestTimeout);
            subscriber3.WaitForConnection(TestTimeout);

            subscriber1.Subscribe(observer1);
            subscriber2.Subscribe(observer2);
            subscriber3.Subscribe(observer3);
            obs.Subscribe(publisher);

            scheduler.AdvanceBy(10);
            observer1.Timeout(TestTimeout).Wait().data.Is("abc");
            observer2.Timeout(TestTimeout).Wait().data.Is("abc");
            observer3.Timeout(TestTimeout).Wait().data.Is("abc");

            observer1.Dispose();
            scheduler.AdvanceBy(10);
            AssertEx.Throws<TimeoutException>(() => observer1.Timeout(TestTimeout).Wait());
            observer2.Timeout(TestTimeout).Wait().data.Is("defg");
            observer3.Timeout(TestTimeout).Wait().data.Is("defg");

            observer2.Dispose();
            scheduler.AdvanceBy(10);
            AssertEx.Throws<TimeoutException>(() => observer1.Timeout(TestTimeout).Wait());
            AssertEx.Throws<TimeoutException>(() => observer2.Timeout(TestTimeout).Wait());
            observer3.Timeout(TestTimeout).Wait().data.Is("hijklmn");


            subscriber1.Dispose();
            subscriber2.Dispose();
            subscriber3.Dispose();
            publisher.Dispose();

            node1.Dispose();
            node2.Dispose();
            node3.Dispose();
        }

        [TestMethod]
        public void MultiplePublisher()
        {
            var scheduler = new TestScheduler();

            var observer = new OneLineCacheSubject<std_msgs.String>();

            var obs1 = scheduler.CreateHotObservable(
                OnNext(110, new std_msgs.String() { data = "abc1" }),
                OnNext(210, new std_msgs.String() { data = "defg1" }),
                OnNext(310, new std_msgs.String() { data = "hijklmn1" })
                );
            var obs2 = scheduler.CreateHotObservable(
                OnNext(120, new std_msgs.String() { data = "abc2" }),
                OnNext(220, new std_msgs.String() { data = "defg2" }),
                OnNext(320, new std_msgs.String() { data = "hijklmn2" })
                );
            var obs3 = scheduler.CreateHotObservable(
                OnNext(130, new std_msgs.String() { data = "abc3" }),
                OnNext(230, new std_msgs.String() { data = "defg3" }),
                OnNext(330, new std_msgs.String() { data = "hijklmn3" })
                );

            var node1 = Ros.InitNodeAsync("test1").Result;
            var node2 = Ros.InitNodeAsync("test2").Result;
            var node3 = Ros.InitNodeAsync("test3").Result;

            var publisher1 = node1.PublisherAsync<std_msgs.String>("test_topic").Result;
            var publisher2 = node2.PublisherAsync<std_msgs.String>("test_topic").Result;
            var subscriber = node1.SubscriberAsync<std_msgs.String>("test_topic").Result;
            var publisher3 = node3.PublisherAsync<std_msgs.String>("test_topic").Result;

            publisher1.WaitForConnection(TestTimeout);
            publisher2.WaitForConnection(TestTimeout);
            publisher3.WaitForConnection(TestTimeout);
            subscriber.WaitForConnection(TestTimeout);

            subscriber.Subscribe(observer);
            obs1.Subscribe(publisher1);
            obs2.Subscribe(publisher2);
            obs3.Subscribe(publisher3);

            scheduler.AdvanceTo(110);
            observer.Timeout(TestTimeout).Wait().data.Is("abc1");

            scheduler.AdvanceTo(120);
            observer.Timeout(TestTimeout).Wait().data.Is("abc2");

            scheduler.AdvanceTo(130);
            observer.Timeout(TestTimeout).Wait().data.Is("abc3");

            scheduler.AdvanceTo(210);
            observer.Timeout(TestTimeout).Wait().data.Is("defg1");

            scheduler.AdvanceTo(220);
            observer.Timeout(TestTimeout).Wait().data.Is("defg2");

            scheduler.AdvanceTo(230);
            observer.Timeout(TestTimeout).Wait().data.Is("defg3");

            scheduler.AdvanceTo(310);
            observer.Timeout(TestTimeout).Wait().data.Is("hijklmn1");

            scheduler.AdvanceTo(320);
            observer.Timeout(TestTimeout).Wait().data.Is("hijklmn2");

            scheduler.AdvanceTo(330);
            observer.Timeout(TestTimeout).Wait().data.Is("hijklmn3");


            subscriber.Dispose();
            publisher1.Dispose();
            publisher2.Dispose();
            publisher3.Dispose();

            node1.Dispose();
            node2.Dispose();
            node3.Dispose();
        }

        [TestMethod]
        public void Unregister()
        {

        }
    }
}
