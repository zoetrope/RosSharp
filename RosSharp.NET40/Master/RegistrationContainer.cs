using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RosSharp.Master
{
    internal class RegistrationContainer
    {
        private Dictionary<string, TopicRegistrationInfo> _topics = new Dictionary<string, TopicRegistrationInfo>();
        
        public List<Uri> RegsiterSubscriber(string topic, string topicType, Uri slaveUri)
        {
            TopicRegistrationInfo info;
            if(_topics.ContainsKey(topic))
            {
                info = _topics[topic];
            }
            else
            {
                info = new TopicRegistrationInfo()
                {
                    TopicName = topic,
                    TopicType = topicType
                };
                _topics.Add(topic, info);
            }

            if(!info.SubscriberUris.Contains(slaveUri))
            {
                info.SubscriberUris.Add(slaveUri);
            }

            return info.PublisherUris;
        }

        public void UnregisterSubscriber()
        {
            
        }

        public List<Uri> RegisterPublisher(string topic, string topicType, Uri slaveUri)
        {
            TopicRegistrationInfo info;
            if (_topics.ContainsKey(topic))
            {
                info = _topics[topic];
            }
            else
            {
                info = new TopicRegistrationInfo()
                {
                    TopicName = topic,
                    TopicType = topicType
                };
                _topics.Add(topic, info);
            }

            if (!info.PublisherUris.Contains(slaveUri))
            {
                info.PublisherUris.Add(slaveUri);
            }

            return info.SubscriberUris;
        }

        public void UnregisterPublisher()
        {
            
        }

    }


    internal class TopicRegistrationInfo
    {
        public TopicRegistrationInfo()
        {
            SubscriberUris = new List<Uri>();
            PublisherUris = new List<Uri>();
        }

        public string TopicName { get; set; }
        public string TopicType { get; set; }

        public List<Uri> SubscriberUris { get; private set; }
        public List<Uri> PublisherUris { get; private set; }
    }
}
