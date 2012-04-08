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
            RosManager.Initialize();

            RosManager.GetNodes().Count.Is(0);

            var node = RosManager.CreateNode("test");

            RosManager.GetNodes().Count.Is(1);

            RosManager.Dispose();

            RosManager.GetNodes().Count.Is(0);
        }

        [TestMethod]
        public void DisposeByNode()
        {
            RosManager.Initialize();

            RosManager.GetNodes().Count.Is(0);

            var node = RosManager.CreateNode("test");

            RosManager.GetNodes().Count.Is(1);

            node.Dispose();

            RosManager.GetNodes().Count.Is(0);

            RosManager.Dispose();

            RosManager.GetNodes().Count.Is(0);
        }
    }
}
