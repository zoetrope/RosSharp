using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using RosSharp.Topic;

namespace RosSharp.MemoryLeakTest
{
    class SubscriberTest : ITest
    {
        private Node _node;
        private Publisher<std_msgs.Int32> _publisher;

        public void Initialize()
        {
            _node = Ros.InitNodeAsync("test", enableLogger: false).Result;

            _publisher = _node.PublisherAsync<std_msgs.Int32>("test").Result;

        }

        public void Do(int index)
        {
            var subscriber = _node.SubscriberAsync<std_msgs.Int32>("test").Result;

            subscriber.WaitForConnection(TimeSpan.FromSeconds(3));
            _publisher.WaitForConnection(TimeSpan.FromSeconds(3));

            var subject = new Subject<std_msgs.Int32>();
            var d = subscriber.Subscribe(subject);

            for (int i = 0; i < 10; i++)
            {
                _publisher.OnNext(new std_msgs.Int32() { data = i });
            }

            d.Dispose();

            subscriber.Dispose();

            _publisher.WaitForDisconnection(TimeSpan.FromSeconds(3));

        }

        public void Cleanup()
        {
            _publisher.Dispose();
            _node.Dispose();
        }
    }
}
