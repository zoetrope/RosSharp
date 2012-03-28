using System;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RosSharp.Topic;

namespace RosSharp.Tests.Topic
{
    [TestClass]
    public class RosTopicClientTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            var topic = new RosTopicClient<std_msgs.String>("mynode","mytopic");
            topic.Start(sock);
            
        }
    }
}
