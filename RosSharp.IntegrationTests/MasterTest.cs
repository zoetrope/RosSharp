using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RosSharp.Master;

namespace RosSharp.IntegrationTests
{
    [TestClass]
    public class MasterTest
    {
        private MasterServer _masterServer;
        private MasterClient _masterClient;

        [TestInitialize]
        public void Initialize()
        {
            Ros.MasterUri = new Uri("http://localhost:11311/");
            Ros.HostName = "localhost";
            Ros.TopicTimeout = 3000;
            Ros.XmlRpcTimeout = 3000;

            _masterServer = new MasterServer(11311);

            _masterClient = new MasterClient(_masterServer.MasterUri);
        }
        
        [TestMethod]
        public void GetSystemState_Empty()
        {
            var state = _masterClient.GetSystemStateAsync("test").Result;

            state.Publishers.Count.Is(0);
            state.Subscribers.Count.Is(0);
            state.Services.Count.Is(0);

        }

        [TestMethod]
        public void GetSystemState_Success()
        {
            _masterClient.RegisterPublisherAsync("test", "test", "std_msgs/String", new Uri("http://localhost:9999")).Wait();

            var state1 = _masterClient.GetSystemStateAsync("test").Result;

            state1.Publishers.Count.Is(1);
            state1.Subscribers.Count.Is(0);
            state1.Services.Count.Is(0);

            _masterClient.RegisterSubscriberAsync("test", "test", "std_msgs/String", new Uri("http://localhost:9999")).Wait();

            var state2 = _masterClient.GetSystemStateAsync("test").Result;
            state2.Publishers.Count.Is(1);
            state2.Subscribers.Count.Is(1);
            state2.Services.Count.Is(0);

            _masterClient.RegisterServiceAsync("test", "test", new Uri("http://localhost:9999"), new Uri("http://localhost:9999")).Wait();

            var state3 = _masterClient.GetSystemStateAsync("test").Result;
            state3.Publishers.Count.Is(1);
            state3.Subscribers.Count.Is(1);
            state3.Services.Count.Is(1);

        }
    }
}
