using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RosSharp.Node;

namespace RosSharp.MemoryLeakTest
{
    class ServiceProxyTest : ITest
    {
        private INode _node;

        public void Initialize()
        {
            _node = Ros.CreateNodeAsync("test", enableLogger: false).Result;

            _node.RegisterServiceAsync("add_two_ints",
                                       new AddTwoInts(x => new AddTwoInts.Response() {sum = x.a + x.b}));
        }

        public void Do(int index)
        {
            var proxy = _node.CreateProxyAsync<AddTwoInts>("add_two_ints").Result;

            for (int i = 0; i < 10; i++)
            {
                proxy.Invoke(new AddTwoInts.Request() {a = i, b = i*2});
            }

            proxy.Dispose();
        }

        public void Cleanup()
        {
            _node.Dispose();
        }
    }
}
