using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RosSharp.MemoryLeakTest
{
    class NodeTest : ITest
    {
        public void Initialize()
        {
            
        }

        public void Do(int index)
        {
            var nodeName = "test" + index;
            var node = Ros.InitNodeAsync(nodeName, enableLogger: false).Result;
            node.Dispose();
        }

        public void Cleanup()
        {
            
        }
    }
}
