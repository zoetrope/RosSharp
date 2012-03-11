using System;
using System.Diagnostics;
using System.Security.Policy;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RosSharp.Slave;
using RosSharp.Topic;

namespace RosSharp.Tests.Slave
{
    [TestClass]
    public class SlaveServerTest
    {
        private SlaveServer _slaveServer;

        [TestInitialize]
        public void Initialize()
        {
            ROS.Initialize();
            var topicContainer = new TopicContainer();
            topicContainer.AddPublisher(new Publisher<StdMsgs.String>("/test_topic"));

            var rosTopicServer = new RosTopicServer();
            _slaveServer = new SlaveServer(topicContainer, rosTopicServer);
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
            ret[0].Is(1);
            ret[2].Is(new Uri("http://192.168.11.4:11311/"));
        }

        [TestMethod]
        public void GetPid_Success()
        {
            var ret = _slaveServer.GetPid("/test");
            ret.Length.Is(3);
            ret[0].Is(1);
            ret[2].Is(Process.GetCurrentProcess().Id);
        }
        
        [TestMethod]
        public void RequestTopic_Success()
        {
            var ret = _slaveServer.RequestTopic("/test", "/test_topic", new object[1] { new string[1] { "TCPROS" } });

            ret.Length.Is(3);
            ret[0].Is(1);
        }

        [TestMethod]
        public void RequestTopic_NoSupportedProtocol()
        {
            var ret = _slaveServer.RequestTopic("/test", "/test_topic", new object[1] {new string[1] {"UDPROS"}});

            ret.Length.Is(3);
            ret[0].Is(-1);
            ret[1].Is("No supported protocols specified.");
        }
        
        [TestMethod]
        public void RequestTopic_NoPublisher()
        {
            var ret = _slaveServer.RequestTopic("/test", "/hoge", new object[1] {new string[1] {"TCPROS"}});
            
            ret.Length.Is(3);
            ret[0].Is(-1);
            ret[1].Is("No publishers for topic: /hoge");
        }
    }
}
