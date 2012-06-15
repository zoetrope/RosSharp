using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RosSharp.Node;
using System.Reactive.Linq;

namespace RosSharp.MemoryLeakTest
{
    class ParameterTest : ITest
    {
        private RosNode _node;

        public void Initialize()
        {
            _node = Ros.InitNodeAsync("test", enableLogger: false).Result;
        }

        public void Do(int index)
        {
            var param = _node.PrimitiveParameterAsync<int>("param" + index).Result;

            var d = param.Subscribe(x =>
            {
                
            });

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
