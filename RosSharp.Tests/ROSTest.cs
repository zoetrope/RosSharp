using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RosSharp.Tests
{
    [TestClass]
    public class ROSTest
    {
        [TestMethod]
        public void Dispose()
        {
            ROS.Initialize();

            ROS.GetNodes().Count.Is(0);

            var node = ROS.CreateNode("test");

            ROS.GetNodes().Count.Is(1);

            ROS.Dispose();

            ROS.GetNodes().Count.Is(0);
        }

        [TestMethod]
        public void DisposeByNode()
        {
            ROS.Initialize();

            ROS.GetNodes().Count.Is(0);

            var node = ROS.CreateNode("test");

            ROS.GetNodes().Count.Is(1);

            node.Dispose();

            ROS.GetNodes().Count.Is(0);

            ROS.Dispose();

            ROS.GetNodes().Count.Is(0);
        }
    }
}
