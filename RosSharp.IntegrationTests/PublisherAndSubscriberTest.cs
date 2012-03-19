using System;
using System.Reactive.Subjects;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RosSharp.Master;
using RosSharp.std_msgs;
using RosSharp.Topic;
using System.Reactive.Linq;
using System.Reactive;

namespace RosSharp.IntegrationTests
{
    [TestClass]
    public class PublisherAndSubscriberTest : ReactiveTest
    {
        private MasterServer _masterServer;

        [TestInitialize]
        public void Initialize()
        {
            ROS.Initialize(new Uri("http://localhost:11311/"), "localhost");
            _masterServer = new MasterServer(11311);
        }

        [TestCleanup]
        public void Cleanup()
        {
            //_masterServer.Dispose();
        }

        [TestMethod]
        public void PublishAndSubscribe()
        {
            var scheduler = new TestScheduler();
            
            var observer = new ReplaySubject<std_msgs.String>();

            var obs = scheduler.CreateHotObservable(
                OnNext(10, new std_msgs.String() {data = "abc"}),
                OnNext(20, new std_msgs.String() {data = "defg"}),
                OnNext(30, new std_msgs.String() {data = "hijklmn"})
                );

            var node = ROS.CreateNode("test");

            var publisher = node.CreatePublisher<std_msgs.String>("test_topic");
            var subscriber = node.CreateSubscriber<std_msgs.String>("test_topic");

            subscriber.Subscribe(observer);
            obs.Subscribe(publisher);

            scheduler.AdvanceBy(10);
            observer.Timeout(TimeSpan.FromSeconds(1)).First().Is(new std_msgs.String() {data = "abc"});

            scheduler.AdvanceBy(10);
            observer.Skip(1).Timeout(TimeSpan.FromSeconds(1)).First().Is(new std_msgs.String() { data = "defg" });
            
            scheduler.AdvanceBy(10);
            observer.Skip(2).Timeout(TimeSpan.FromSeconds(1)).First().Is(new std_msgs.String() { data = "hijklmn" });
            
        }

        [TestMethod]
        public void SubscribeAndPublish()
        {
            var scheduler = new TestScheduler();

            var observer = new ReplaySubject<std_msgs.String>();

            var obs = scheduler.CreateHotObservable(
                OnNext(10, new std_msgs.String() { data = "abc" }),
                OnNext(20, new std_msgs.String() { data = "defg" }),
                OnNext(30, new std_msgs.String() { data = "hijklmn" })
                );

            var node = ROS.CreateNode("test");

            var subscriber = node.CreateSubscriber<std_msgs.String>("test_topic");
            var publisher = node.CreatePublisher<std_msgs.String>("test_topic");

            subscriber.Subscribe(observer);
            obs.Subscribe(publisher);

            scheduler.AdvanceBy(10);
            observer.Timeout(TimeSpan.FromSeconds(1)).First().Is(new std_msgs.String() { data = "abc" });

            scheduler.AdvanceBy(10);
            observer.Skip(1).Timeout(TimeSpan.FromSeconds(1)).First().Is(new std_msgs.String() { data = "defg" });

            scheduler.AdvanceBy(10);
            observer.Skip(2).Timeout(TimeSpan.FromSeconds(1)).First().Is(new std_msgs.String() { data = "hijklmn" });
            
        }

        [TestMethod]
        public void MultipleSubscriber()
        {
            
        }

        [TestMethod]
        public void MultiplePublisher()
        {

        }

        [TestMethod]
        public void Unregister()
        {

        }
    }
}
