using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RosSharp.Master
{
    internal sealed class RegistrationContainer
    {
        private readonly Dictionary<string, TopicRegistrationInfo> _topics 
            = new Dictionary<string, TopicRegistrationInfo>();
        private readonly Dictionary<string, Uri> _serviceUris 
            = new Dictionary<string, Uri>();


        public void RegisterService(string service, Uri serviceUri, Uri slaveUri)
        {
            _serviceUris.Add(service, serviceUri);
        }

        public bool UnregisterService(string service, Uri serviceUri)
        {
            throw new NotImplementedException();
        }

        public Uri LookUpService(string service)
        {
            if (_serviceUris.ContainsKey(service))
            {
                return _serviceUris[service];
            }
            else
            {
                return null;
            }
        }

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


        public bool UnregisterSubscriber(string topic, Uri uri)
        {
            throw new NotImplementedException();
        }

        public TopicRegistrationInfo RegisterPublisher(string topic, string topicType, Uri slaveUri)
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

            return info;
        }

        public bool UnregisterPublisher(string topic, Uri uri)
        {
            throw new NotImplementedException();
        }


        public Uri LookUpNode(string nodeName)
        {
            throw new NotImplementedException();
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
