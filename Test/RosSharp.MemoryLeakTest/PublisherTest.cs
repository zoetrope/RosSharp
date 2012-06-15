using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using RosSharp.Node;
using RosSharp.Topic;

namespace RosSharp.MemoryLeakTest
{
    class PublisherTest : ITest
    {
        private RosNode _node;
        private Subscriber<std_msgs.Int32> _subscriber;

        public void Initialize()
        {
            _node = (RosNode)Ros.InitNodeAsync("test", enableLogger: false).Result;

            _subscriber = _node.SubscriberAsync<std_msgs.Int32>("test").Result;
        }

        public void Do(int index)
        {
            var publisher = _node.PublisherAsync<std_msgs.Int32>("test").Result;

            publisher.WaitForConnection(TimeSpan.FromSeconds(3));
            _subscriber.WaitForConnection(TimeSpan.FromSeconds(3));

            var subject = new Subject<std_msgs.Int32>();
            var d = _subscriber.Subscribe(subject);

            for (int i = 0; i < 10; i++)
            {
                publisher.OnNext(new std_msgs.Int32() {data = i});
            }

            d.Dispose();

            publisher.Dispose();

            _subscriber.WaitForDisconnection(TimeSpan.FromSeconds(3));
            
        }

        public void Cleanup()
        {
            _subscriber.Dispose();
            _node.Dispose();
        }
    }
}
