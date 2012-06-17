using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RosSharp.MemoryLeakTest
{
    class ServiceServerTest : ITest
    {
        private Node _node;

        public void Initialize()
        {
            _node = Ros.InitNodeAsync("test", enableLogger: false).Result;

        }

        public void Do(int index)
        {
            var d = _node
                .AdvertiseServiceAsync("add_two_ints", new AddTwoInts(x => new AddTwoInts.Response() {sum = x.a + x.b}))
                .Result;


            var proxy = _node.ServiceProxyAsync<AddTwoInts>("add_two_ints").Result;

            for (int i = 0; i < 10; i++)
            {
                proxy.Invoke(new AddTwoInts.Request() { a = i, b = i * 2 });
            }

            proxy.Dispose();
            
            d.Dispose();
        }

        public void Cleanup()
        {
            _node.Dispose();
        }
    }
}
