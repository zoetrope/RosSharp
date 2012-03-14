using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RosSharp.Master;
using RosSharp.StdMsgs;
using RosSharp.Topic;

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
            var mock = scheduler.CreateObserver<StdMsgs.String>();
            var obs = scheduler.CreateHotObservable(
                OnNext(10, new StdMsgs.String() {data = "abc"}),
                OnNext(20, new StdMsgs.String() {data = "defg"}),
                OnNext(30, new StdMsgs.String() {data = "hijklmn"})
                );

            var node = ROS.CreateNode("test");

            var publisher = node.CreatePublisher<StdMsgs.String>("test_topic");
            var subscriber = node.CreateSubscriber<StdMsgs.String>("test_topic");

            subscriber.Subscribe(mock);
            obs.Subscribe(publisher);

            scheduler.AdvanceTo(50);

            //TODO: 裏で通信を行っているので、データがそろうまで待つ必要がある。
            mock.Messages.Count.Is(3);
            mock.Messages[0].Value.Value.data.Is("abc");
            mock.Messages[1].Value.Value.data.Is("defg");
            mock.Messages[2].Value.Value.data.Is("hijklmn");
        }

        [TestMethod]
        public void SubscribeAndPublish()
        {
            var scheduler = new TestScheduler();
            var mock = scheduler.CreateObserver<StdMsgs.String>();
            var obs = scheduler.CreateHotObservable(
                OnNext(10, new StdMsgs.String() { data = "abc" }),
                OnNext(20, new StdMsgs.String() { data = "defg" }),
                OnNext(30, new StdMsgs.String() { data = "hijklmn" })
                );

            var node = ROS.CreateNode("test");

            var subscriber = node.CreateSubscriber<StdMsgs.String>("test_topic");
            var publisher = node.CreatePublisher<StdMsgs.String>("test_topic");

            subscriber.Subscribe(mock);
            obs.Subscribe(publisher);

            scheduler.AdvanceTo(50);

            mock.Messages.Count.Is(3);
            mock.Messages[0].Value.Value.data.Is("abc");
            mock.Messages[1].Value.Value.data.Is("defg");
            mock.Messages[2].Value.Value.data.Is("hijklmn");
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
