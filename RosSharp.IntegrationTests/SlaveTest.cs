using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RosSharp.Slave;
using RosSharp.Topic;
using RosSharp.Transport;

namespace RosSharp.IntegrationTests
{
    [TestClass]
    public class SlaveTest
    {

        private SlaveServer _slaveServer;
        private TopicContainer _container;
        private SlaveClient _slaveClient;

        [TestInitialize]
        public void Initialize()
        {
            Ros.MasterUri = new Uri("http://localhost:11311/");
            Ros.HostName = "localhost";
            Ros.TopicTimeout = 3000;
            Ros.XmlRpcTimeout = 3000;

            _container = new TopicContainer();
            var listener = new TcpRosListener(0);

            _slaveServer = new SlaveServer("test", 0, _container, listener);

            _slaveClient = new SlaveClient(_slaveServer.SlaveUri);
        }


        [TestMethod]
        public void TestMethod1()
        {

            var stats = _slaveClient.GetBusStatsAsync("test").Result;
            
        }
    }
}
