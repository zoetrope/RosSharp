using System;
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
            var topicContainer = new TopicContainer();
            _slaveServer = new SlaveServer(new Uri("http://localhost:55555/"),topicContainer);
        }

        [TestMethod]
        public void GetPublications_Empty()
        {
            _slaveServer.GetPublications("/notfound");
        }
    }
}
