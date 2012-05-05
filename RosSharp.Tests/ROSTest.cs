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
            Ros.GetNodes().Count.Is(0);

            var node = Ros.CreateNode("test");

            Ros.GetNodes().Count.Is(1);

            Ros.Dispose();

            Ros.GetNodes().Count.Is(0);
        }

        [TestMethod]
        public void DisposeByNode()
        {
            Ros.GetNodes().Count.Is(0);

            var node = Ros.CreateNode("test");

            Ros.GetNodes().Count.Is(1);

            node.Dispose();

            Ros.GetNodes().Count.Is(0);

            Ros.Dispose();

            Ros.GetNodes().Count.Is(0);
        }
    }
}
