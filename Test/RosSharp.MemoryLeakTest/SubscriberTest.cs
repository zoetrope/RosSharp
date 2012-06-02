using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RosSharp.Node;
using RosSharp.Topic;

namespace RosSharp.MemoryLeakTest
{
    class SubscriberTest : ITest
    {
        private INode _node;
        private Publisher<std_msgs.Int32> _publisher;

        public void Initialize()
        {
            _node = Ros.CreateNodeAsync("test", enableLogger: false).Result;

            _publisher = _node.CreatePublisherAsync<std_msgs.Int32>("test").Result;

        }

        public void Do(int index)
        {
            var subscriber = _node.CreateSubscriberAsync<std_msgs.Int32>("test").Result;
            for (int i = 0; i < 10; i++)
            {
                _publisher.OnNext(new std_msgs.Int32() { data = i });
            }

            subscriber.Dispose();
        }

        public void Cleanup()
        {
            _publisher.Dispose();
            _node.Dispose();
        }
    }
}
