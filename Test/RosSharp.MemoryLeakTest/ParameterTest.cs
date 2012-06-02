using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RosSharp.Node;

namespace RosSharp.MemoryLeakTest
{
    class ParameterTest : ITest
    {
        private INode _node;

        public void Initialize()
        {
            _node = Ros.CreateNodeAsync("test", enableLogger: false).Result;
        }

        public void Do(int index)
        {
            var param = _node.CreateParameterAsync<int>("param" + index).Result;

            var d = param.Subscribe(x => Console.WriteLine(x));

            for (int i = 0; i < 10;i++ )
            {
                param.Value = i;
            }

            d.Dispose();

            param.Dispose();
        }

        public void Cleanup()
        {
            _node.Dispose();
        }
    }
}
