using System;
using System.Reactive.Linq;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Reactive.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RosSharp.Master;
using Server;

namespace RosSharp.IntegrationTests
{
    [TestClass]
    public class ServerAndClientTest : ReactiveTest
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
        public void ServerAndProxy()
        {
            var node = ROS.CreateNode("test");
            node.RegisterService<AddTwoInts, AddTwoInts.Request, AddTwoInts.Response>("/add_two_ints", add_two_ints);
            var proxy = node.CreateProxy<AddTwoInts, AddTwoInts.Request, AddTwoInts.Response>("/add_two_ints");
            var ret = proxy(new AddTwoInts.Request() {a = 1, b = 2}).First();
            ret.c.Is(3);
        }


        static AddTwoInts.Response add_two_ints(AddTwoInts.Request req)
        {
            return new AddTwoInts.Response() { c = req.a + req.b };
        }
    }
}
