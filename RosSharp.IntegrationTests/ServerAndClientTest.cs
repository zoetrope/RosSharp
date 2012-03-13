using System;
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
            proxy(new AddTwoInts.Request() { a = 1, b = 2 }).Subscribe(x => Console.WriteLine(x.c));
        }


        static AddTwoInts.Response add_two_ints(AddTwoInts.Request req)
        {
            Console.WriteLine("add_two_ints: a = {0}, b = {1}", req.a, req.b);
            return new AddTwoInts.Response() { c = req.a + req.b };
        }
    }
}
