using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RosSharp.Message;

namespace RosSharp.Topic
{
    public sealed class TopicContainer
    {
        private readonly ConcurrentDictionary<string, IPublisher> _publishers = new ConcurrentDictionary<string, IPublisher>();
        private readonly ConcurrentDictionary<string, ISubscriber> _subscribers = new ConcurrentDictionary<string, ISubscriber>();

        public bool AddPublisher<TDataType>(Publisher<TDataType> publisher)
            where TDataType : IMessage, new()
        {
            return _publishers.TryAdd(publisher.Name, publisher);
        }

        public bool RemovePublisher(string topicName)
        {
            IPublisher dummy;
            return _publishers.TryRemove(topicName, out dummy);
        }
        
        public bool AddSubscriber<TDataType>(Subscriber<TDataType> subscriber)
            where TDataType : IMessage, new()
        {
            return _subscribers.TryAdd(subscriber.Name, subscriber);
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
