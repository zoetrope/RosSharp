using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RosSharp.Master;
using RosSharp.StdMsgs;
using RosSharp.Topic;

namespace RosSharp.IntegrationTests
{
    [TestClass]
    public class PublisherAndSubscriberTest
    {
        private MasterServer _masterServer;
        private Publisher<StdMsgs.String> _publisher;
        private Subscriber<StdMsgs.String> _subscriber;

        [TestInitialize]
        public void Initialize()
        {
            ROS.Initialize(new Uri("http://localhost:11311/"), "localhost");

            _masterServer = new MasterServer(11311);
            var node = ROS.CreateNode("test");

            _publisher = node.CreatePublisher<StdMsgs.String>("test_topic");
            _subscriber = node.CreateSubscriber<StdMsgs.String>("test_topic");
        }

        [TestMethod]
        public void TestMethod1()
        {
            _subscriber.Subscribe(x => Console.WriteLine(x.data));

            _publisher.OnNext(new StdMsgs.String(){data = "test"});


        }
    }
}
