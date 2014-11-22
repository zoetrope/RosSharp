using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
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
            Ros.MasterUri = new Uri("http://localhost:11311/");
            Ros.HostName = "localhost";
            Ros.TopicTimeout = 3000;
            Ros.XmlRpcTimeout = 3000;

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
            var node = Ros.InitNodeAsync("test").Result;
            node.AdvertiseServiceAsync("/add_two_ints", new AddTwoInts(add_two_ints)).Wait();

            var proxy = node.ServiceProxyAsync<AddTwoInts>("/add_two_ints").Result;
            
            var res = proxy.InvokeAsync(new AddTwoInts.Request() {a = 1, b = 2})
                .ToObservable()
                .Timeout(TimeSpan.FromSeconds(3))
                .Wait();

            res.sum.Is(3);
        }


        static AddTwoInts.Response add_two_ints(AddTwoInts.Request req)
        {
            Console.WriteLine("a={0},b={1}", req.a, req.b);
            return new AddTwoInts.Response() { sum = req.a + req.b };
        }
    }
}
