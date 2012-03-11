using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RosSharp.Message;

namespace RosSharp.Topic
{
    public class TopicContainer
    {
        private readonly Dictionary<string, IPublisher> _publishers = new Dictionary<string, IPublisher>();
        private readonly Dictionary<string, ISubscriber> _subscribers = new Dictionary<string, ISubscriber>();

        public void AddPublisher<TDataType>(Publisher<TDataType> publisher)
            where TDataType : IMessage, new()
        {
            _publishers.Add(publisher.Name, publisher);
        }
        
        public void AddSubscriber<TDataType>(Subscriber<TDataType> subscriber)
            where TDataType : IMessage, new()
        {
            _subscribers.Add(subscriber.Name, subscriber);
        }

        public List<IPublisher> GetPublishers()
        {
            return _publishers.Values.ToList();
        }

        public List<ISubscriber> GetSubscribers()
        {
            return _subscribers.Values.ToList();
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
