using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RosSharp.Message;

namespace RosSharp.Topic
{
    internal sealed class TopicContainer
    {
        //TODO: concurrentではなく、呼び出す側でロックすべきか。
        private readonly ConcurrentDictionary<string, IPublisher> _publishers = new ConcurrentDictionary<string, IPublisher>();
        private readonly ConcurrentDictionary<string, ISubscriber> _subscribers = new ConcurrentDictionary<string, ISubscriber>();

        public bool AddPublisher(IPublisher publisher)
        {
            return _publishers.TryAdd(publisher.TopicName, publisher);
        }

        public bool RemovePublisher(string topicName)
        {
            IPublisher dummy;
            return _publishers.TryRemove(topicName, out dummy);
        }
        
        public bool AddSubscriber(ISubscriber subscriber)
        {
            return _subscribers.TryAdd(subscriber.TopicName, subscriber);
        }
        
        public bool RemoveSubscriber(string topicName)
        {
            ISubscriber dummy;
            return _subscribers.TryRemove(topicName, out dummy);
        }
        
        public List<IPublisher> GetPublishers()
        {
            return _publishers.Values.ToList();
        }

        public bool GetPublisher(string topicName, out IPublisher publisher)
        {
            return _publishers.TryGetValue(topicName, out publisher);
        }

        public List<ISubscriber> GetSubscribers()
        {
            return _subscribers.Values.ToList();
        }

        public bool GetSubscriber(string topicName, out ISubscriber subscriber)
        {
            return _subscribers.TryGetValue(topicName, out subscriber);
        }

        public bool HasPublisher(string topic)
        {
            return _publishers.ContainsKey(topic);
        }

        public bool HasSubscriber(string topic)
        {
            return _subscribers.ContainsKey(topic);
        }
    }
}
