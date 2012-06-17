using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RosSharp.Master;

namespace RosSharp.Tests.Node
{
    [TestClass]
    public class NodeTest
    {
        private MasterServer _masterServer;

        [TestInitialize]
        public void Initialize()
        {
            Ros.MasterUri = new Uri("http://localhost:11311/");
            Ros.HostName = "localhost";
            Ros.TopicTimeout = 3000;
            Ros.XmlRpcTimeout = 3000;

            _masterServer = new MasterServer(11311);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _masterServer.Dispose();
            Ros.Dispose();
        }


        [TestMethod]
        public void CreateNode_Error()
        {
            Ros.MasterUri = new Uri("http://localhost:9999/");

            AssertEx.Throws<AggregateException>(() => Ros.InitNodeAsync("test").Wait());
        }

        [TestMethod]
        public void CreateSubscriber_Error()
        {
            Ros.MasterUri = new Uri("http://localhost:9999/");
            var node = Ros.InitNodeAsync("test",enableLogger:false).Result;
            AssertEx.Throws<AggregateException>(() => node.SubscriberAsync<std_msgs.String>("test_topic").Wait());
        }

        [TestMethod]
        public void CreatePublisher_Error()
        {
            Ros.MasterUri = new Uri("http://localhost:9999/");
            var node = Ros.InitNodeAsync("test", enableLogger: false).Result;
            AssertEx.Throws<AggregateException>(() => node.PublisherAsync<std_msgs.String>("test_topic").Wait());
        }

        [TestMethod]
        public void RegisterService_Error()
        {
            Ros.MasterUri = new Uri("http://localhost:9999/");
            var node = Ros.InitNodeAsync("test", enableLogger: false).Result;
            AssertEx.Throws<AggregateException>(() => node.AdvertiseServiceAsync("/chatter", new AddTwoInts(_ => new AddTwoInts.Response())).Wait());
        }

        [TestMethod]
        public void CreateProxy_Error()
        {
            Ros.MasterUri = new Uri("http://localhost:9999/");
            var node = Ros.InitNodeAsync("test", enableLogger: false).Result;
            AssertEx.Throws<AggregateException>(() => node.ServiceProxyAsync<AddTwoInts>("/chatter").Wait());
        }

        [TestMethod]
        public void DisposeNode()
        {
            var node = Ros.InitNodeAsync("test").Result;

            var pub = node.PublisherAsync<std_msgs.String>("test_topic").Result;
            var sub = node.SubscriberAsync<std_msgs.String>("test_topic").Result;
            var service = node.AdvertiseServiceAsync(
                "myservice", new AddTwoInts(req => new AddTwoInts.Response() {sum = req.a + req.b}));
            var param = node.PrimitiveParameterAsync<int>("param").Result;

            node.Dispose();

            Ros.GetNodes().Count.Is(0);
       }


        [TestMethod]
        public void DisposePublisher()
        {
            var node = Ros.InitNodeAsync("test").Result;

            var pub = node.PublisherAsync<std_msgs.String>("test_topic").Result;
            var sub = node.SubscriberAsync<std_msgs.String>("test_topic").Result;

            pub.Dispose();
            node.Dispose();
        }
        
        [TestMethod]
        public void DisposeSubscriber()
        {
            var node = Ros.InitNodeAsync("test").Result;

            var pub = node.PublisherAsync<std_msgs.String>("test_topic").Result;
            var sub = node.SubscriberAsync<std_msgs.String>("test_topic").Result;

            sub.Dispose();
            node.Dispose();
        }
        [TestMethod]
        public void DisposeService()
        {
            var node = Ros.InitNodeAsync("test").Result;

            var service = node.AdvertiseServiceAsync(
                "myservice", new AddTwoInts(req => new AddTwoInts.Response() {sum = req.a + req.b})).Result;

            service.Dispose();
        }

        [TestMethod]
        public void DisposeParameter()
        {
            var node = Ros.InitNodeAsync("test").Result;

            var param = node.PrimitiveParameterAsync<int>("param").Result;

            param.Dispose();
        }
    }
}
