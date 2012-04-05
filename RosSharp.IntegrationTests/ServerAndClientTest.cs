using System;
using System.Reactive.Linq;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RosSharp.Master;

namespace RosSharp.IntegrationTests
{
    [TestClass]
    public class ServerAndClientTest : ReactiveTest
    {
        private MasterServer _masterServer;

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
            //_masterServer.Dispose();
        }


        [TestMethod]
        public void ServerAndProxy()
        {
            var node = ROS.CreateNode("test");
            node.RegisterService("/add_two_ints", new AddTwoInts(add_two_ints)).Wait();

            var proxy = node.CreateProxy<AddTwoInts>("/add_two_ints").Result;
            var res = proxy.Invoke(new AddTwoInts.Request() {a = 1, b = 2});
            res.sum.Is(3);
        }


        static AddTwoInts.Response add_two_ints(AddTwoInts.Request req)
        {
            Console.WriteLine("a={0},b={1}", req.a, req.b);
            return new AddTwoInts.Response() { sum = req.a + req.b };
        }
    }
}
