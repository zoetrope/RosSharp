#region License Terms

// ================================================================================
// RosSharp
// 
// Software License Agreement (BSD License)
// 
// Copyright (C) 2012 zoetrope
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// ================================================================================

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;

namespace RosSharp.Master
{
    internal sealed class RegistrationContainer
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();
        private readonly Dictionary<string, Uri> _serviceUris
            = new Dictionary<string, Uri>();

        private readonly Dictionary<string, TopicRegistrationInfo> _topics
            = new Dictionary<string, TopicRegistrationInfo>();


        public void RegisterService(string service, Uri serviceUri, Uri slaveUri)
        {
            if (!_serviceUris.ContainsKey(service))
            {
                _serviceUris.Add(service, serviceUri);
            }
            else
            {
                _logger.Error(m => m("Already registered Service [{0}]", service));
            }
        }

        public bool UnregisterService(string service, Uri serviceUri)
        {
            return _serviceUris.Remove(service);
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

            if (!info.SubscriberUris.Contains(slaveUri))
            {
                info.SubscriberUris.Add(slaveUri);
            }

            return info.PublisherUris;
        }


        public bool UnregisterSubscriber(string topic, Uri uri)
        {
            if(_topics.ContainsKey(topic))
            {
                _topics[topic].SubscriberUris.Remove(uri);

                if (_topics[topic].SubscriberUris.Count == 0 && _topics[topic].PublisherUris.Count == 0)
                {
                    _topics.Remove(topic);
                }
            }

            return true;
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
            if (_topics.ContainsKey(topic))
            {
                _topics[topic].PublisherUris.Remove(uri);

                if (_topics[topic].SubscriberUris.Count == 0 && _topics[topic].PublisherUris.Count == 0)
                {
                    _topics.Remove(topic);
                }
            }

            return true;
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