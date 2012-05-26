using System;
using System.Diagnostics;
using System.Security.Policy;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RosSharp.Slave;
using RosSharp.Topic;
using RosSharp.Transport;

namespace RosSharp.Tests.Slave
{
    [TestClass]
    public class SlaveServerTest
    {
        private SlaveServer _slaveServer;

        [TestInitialize]
        public void Initialize()
        {
            Ros.MasterUri = new Uri("http://localhost:11311/");
            Ros.HostName = "localhost";

            var topicContainer = new TopicContainer();
            topicContainer.AddPublisher(new Publisher<std_msgs.String>("/test_topic", "test"));

            var tcpListener = new TcpRosListener(0);
            _slaveServer = new SlaveServer("test", 0, topicContainer);
            _slaveServer.AddListener("/test_topic", tcpListener);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _slaveServer.Dispose();
            Ros.Dispose();
        }

        [TestMethod]
        public void GetPublications_Empty()
        {
            _slaveServer.GetPublications("/notfound");
        }

        [TestMethod]
        public void GetMasterUri_Success()
        {
            var ret = _slaveServer.GetMasterUri("/test");
            ret.Length.Is(3);
            ret[0].Is(StatusCode.Success);
            ret[2].Is(new Uri("http://localhost:11311/"));
        }

        [TestMethod]
        public void GetPid_Success()
        {
            var ret = _slaveServer.GetPid("/test");
            ret.Length.Is(3);
            ret[0].Is(StatusCode.Success);
            ret[2].Is(Process.GetCurrentProcess().Id);
        }
        
        [TestMethod]
        public void RequestTopic_Success()
        {
            var ret = _slaveServer.RequestTopic("/test", "/test_topic", new object[1] { new string[1] { "TCPROS" } });

            ret.Length.Is(3);
            ret[0].Is(StatusCode.Success);
        }

        [TestMethod]
        public void RequestTopic_NoSupportedProtocol()
        {
            var ret = _slaveServer.RequestTopic("/test", "/test_topic", new object[1] {new string[1] {"UDPROS"}});

            ret.Length.Is(3);
            ret[0].Is(StatusCode.Error);
            ret[1].Is("No supported protocols specified.");
        }
        
        [TestMethod]
        public void RequestTopic_NoPublisher()
        {
            var ret = _slaveServer.RequestTopic("/test", "/hoge", new object[1] {new string[1] {"TCPROS"}});
            
            ret.Length.Is(3);
            ret[0].Is(StatusCode.Error);
            ret[1].Is("No publishers for topic: /hoge");
        }
    }
}
