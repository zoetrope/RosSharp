using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RosSharp.Node;
using RosSharp.Topic;

namespace RosSharp.MemoryLeakTest
{
    class PublisherTest : ITest
    {
        private INode _node;
        private Subscriber<std_msgs.Int32> _subscriber;

        public void Initialize()
        {
            _node = Ros.CreateNodeAsync("test", enableLogger: false).Result;

            _subscriber = _node.CreateSubscriberAsync<std_msgs.Int32>("test").Result;
        }

        public void Do(int index)
        {
            var publisher = _node.CreatePublisherAsync<std_msgs.Int32>("test").Result;
            
            for (int i = 0; i < 10; i++)
            {
                publisher.OnNext(new std_msgs.Int32() {data = i});
            }

            publisher.Dispose();
        }

        public void Cleanup()
        {
            _subscriber.Dispose();
            _node.Dispose();
        }
    }
}
