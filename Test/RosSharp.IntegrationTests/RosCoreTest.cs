using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RosSharp.Master;
using RosSharp.Parameter;

namespace RosSharp.IntegrationTests
{
    [TestClass]
    public class RosCoreTest
    {
        private MasterServer _masterServer;
        private MasterClient _masterClient;
        private ParameterServerClient _parameterServerClient;

        private static TimeSpan TestTimeout = TimeSpan.FromSeconds(3);

        [TestInitialize]
        public void Initialize()
        {
            Ros.MasterUri = new Uri("http://localhost:11311/");
            Ros.HostName = "localhost";
            Ros.TopicTimeout = 1000;
            Ros.XmlRpcTimeout = 1000;

            _masterServer = new MasterServer(11311);

            _masterClient = new MasterClient(_masterServer.MasterUri);
            _parameterServerClient = new ParameterServerClient(_masterServer.MasterUri);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _masterServer.Dispose();
            Ros.Dispose();
        }

        [TestMethod]
        public void TestMethod1()
        {
            var res = _parameterServerClient.UnsubscribeParamAsync("test", new Uri("http://localhost:11111"), "tese").Result;
        }
    }
}
