using System;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RosSharp.Master;
using RosSharp.Transport;

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
            ROS.Initialize();
            ROS.MasterUri = new Uri("http://localhost:11311/");
            ROS.HostName = "localhost";
            ROS.TopicTimeout = 3000;
            ROS.XmlRpcTimeout = 3000;

            _masterServer = new MasterServer(11311);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _masterServer.Dispose();
            ROS.Dispose();
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

            var node = ROS.CreateNode("test");

            var publisher = node.CreatePublisherAsync<std_msgs.String>("test_topic").Result;
            var subscriber = node.CreateSubscriberAsync<std_msgs.String>("test_topic").Result;

            publisher.OnConnectedAsObservable().Timeout(TestTimeout).First();
            subscriber.OnConnectedAsObservable().Timeout(TestTimeout).First();

            subscriber.Subscribe(observer);
            obs.Subscribe(publisher);

            scheduler.AdvanceBy(10);
            observer.Timeout(TestTimeout).First().data.Is("abc");

            scheduler.AdvanceBy(10);
            observer.Timeout(TestTimeout).First().data.Is("defg");
            
            scheduler.AdvanceBy(10);
            observer.Timeout(TestTimeout).First().data.Is("hijklmn");
            
            

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

            var node = ROS.CreateNode("test");

            var subscriber = node.CreateSubscriberAsync<std_msgs.String>("test_topic").Result;
            var publisher = node.CreatePublisherAsync<std_msgs.String>("test_topic").Result;

            publisher.OnConnectedAsObservable().Timeout(TestTimeout).First();
            subscriber.OnConnectedAsObservable().Timeout(TestTimeout).First();

            subscriber.Subscribe(observer);
            obs.Subscribe(publisher);

            scheduler.AdvanceBy(10);
            observer.Timeout(TestTimeout).First().data.Is("abc");

            scheduler.AdvanceBy(10);
            observer.Timeout(TestTimeout).First().data.Is("defg");

            scheduler.AdvanceBy(10);
            observer.Timeout(TestTimeout).First().data.Is("hijklmn");


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

            var node1 = ROS.CreateNode("test1");
            var node2 = ROS.CreateNode("test2");
            var node3 = ROS.CreateNode("test3");

            var subscriber1 = node1.CreateSubscriberAsync<std_msgs.String>("test_topic").Result;
            var subscriber2 = node2.CreateSubscriberAsync<std_msgs.String>("test_topic").Result;
            var publisher = node1.CreatePublisherAsync<std_msgs.String>("test_topic").Result;
            var subscriber3 = node3.CreateSubscriberAsync<std_msgs.String>("test_topic").Result;

            publisher.OnConnectedAsObservable().Timeout(TestTimeout).First();
            subscriber1.OnConnectedAsObservable().Timeout(TestTimeout).First();
            subscriber2.OnConnectedAsObservable().Timeout(TestTimeout).First();
            subscriber3.OnConnectedAsObservable().Timeout(TestTimeout).First();

            subscriber1.Subscribe(observer1);
            subscriber2.Subscribe(observer2);
            subscriber3.Subscribe(observer3);
            obs.Subscribe(publisher);

            scheduler.AdvanceBy(10);
            observer1.Timeout(TestTimeout).First().data.Is("abc");
            observer2.Timeout(TestTimeout).First().data.Is("abc");
            observer3.Timeout(TestTimeout).First().data.Is("abc");

            scheduler.AdvanceBy(10);
            observer1.Timeout(TestTimeout).First().data.Is("defg");
            observer2.Timeout(TestTimeout).First().data.Is("defg");
            observer3.Timeout(TestTimeout).First().data.Is("defg");

            scheduler.AdvanceBy(10);
            observer1.Timeout(TestTimeout).First().data.Is("hijklmn");
            observer2.Timeout(TestTimeout).First().data.Is("hijklmn");
            observer3.Timeout(TestTimeout).First().data.Is("hijklmn");


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

            var node1 = ROS.CreateNode("test1");
            var node2 = ROS.CreateNode("test2");
            var node3 = ROS.CreateNode("test3");

            var publisher1 = node1.CreatePublisherAsync<std_msgs.String>("test_topic").Result;
            var publisher2 = node2.CreatePublisherAsync<std_msgs.String>("test_topic").Result;
            var subscriber = node1.CreateSubscriberAsync<std_msgs.String>("test_topic").Result;
            var publisher3 = node3.CreatePublisherAsync<std_msgs.String>("test_topic").Result;

            publisher1.OnConnectedAsObservable().Timeout(TestTimeout).First();
            publisher2.OnConnectedAsObservable().Timeout(TestTimeout).First();
            publisher3.OnConnectedAsObservable().Timeout(TestTimeout).First();
            subscriber.OnConnectedAsObservable().Timeout(TestTimeout).First();

            subscriber.Subscribe(observer);
            obs1.Subscribe(publisher1);
            obs2.Subscribe(publisher2);
            obs3.Subscribe(publisher3);

            scheduler.AdvanceTo(110);
            observer.Timeout(TestTimeout).First().data.Is("abc1");

            scheduler.AdvanceTo(120);
            observer.Timeout(TestTimeout).First().data.Is("abc2");

            scheduler.AdvanceTo(130);
            observer.Timeout(TestTimeout).First().data.Is("abc3");

            scheduler.AdvanceTo(210);
            observer.Timeout(TestTimeout).First().data.Is("defg1");

            scheduler.AdvanceTo(220);
            observer.Timeout(TestTimeout).First().data.Is("defg2");

            scheduler.AdvanceTo(230);
            observer.Timeout(TestTimeout).First().data.Is("defg3");

            scheduler.AdvanceTo(310);
            observer.Timeout(TestTimeout).First().data.Is("hijklmn1");

            scheduler.AdvanceTo(320);
            observer.Timeout(TestTimeout).First().data.Is("hijklmn2");

            scheduler.AdvanceTo(330);
            observer.Timeout(TestTimeout).First().data.Is("hijklmn3");


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
