using System;
using System.Reactive.Linq;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RosSharp.Master;
using RosSharp.Parameter;

namespace RosSharp.IntegrationTests
{
    [TestClass]
    public class ParameterServerTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            ROS.Initialize();
            ROS.HostName = "localhost";

            var masterServer = new MasterServer(11311);

            var masterClient = new MasterClient(new Uri("http://localhost:11311"));

            var parameterClient = new ParameterServerClient(new Uri("http://localhost:11311"));

            parameterClient.GetParamAsync("", "").First();
        }
    }
}
