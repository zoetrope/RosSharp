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
        private readonly Dictionary<string, ServiceRegistrationInfo> _serviceRegistrationInfos
            = new Dictionary<string, ServiceRegistrationInfo>();

        private readonly Dictionary<string, TopicRegistrationInfo> _topicRegistrationInfos
            = new Dictionary<string, TopicRegistrationInfo>();


        public void RegisterService(string nodeId, string service, Uri serviceUri, Uri slaveUri)
        {
            if (!_serviceRegistrationInfos.ContainsKey(service))
            {
                _serviceRegistrationInfos.Add(service, new ServiceRegistrationInfo()
                {
                    ServiceName = service,
                    Service = new RegistrationInfo() {NodeId = nodeId, Uri = serviceUri}
                });
            }
            else
            {
                _logger.Error(m => m("Already registered Service [{0}]", service));
            }
        }

        public bool UnregisterService(string service, Uri serviceUri)
        {
            return _serviceRegistrationInfos.Remove(service);
        }

        public ServiceRegistrationInfo LookUpService(string service)
        {
            if (_serviceRegistrationInfos.ContainsKey(service))
            {
                return _serviceRegistrationInfos[service];
            }
            else
            {
                return null;
            }
        }

        public List<RegistrationInfo> RegsiterSubscriber(string nodeId, string topic, string topicType, Uri slaveUri)
        {
            TopicRegistrationInfo info;
            if (_topicRegistrationInfos.ContainsKey(topic))
            {
                info = _topicRegistrationInfos[topic];
            }
            else
            {
                info = new TopicRegistrationInfo()
                {
                    TopicName = topic,
                    TopicType = topicType
                };
                _topicRegistrationInfos.Add(topic, info);
            }

            if (!info.Subscribers.Any(x=>x.NodeId == nodeId && x.Uri == slaveUri))
            {
                info.Subscribers.Add(new RegistrationInfo() {NodeId = nodeId, Uri = slaveUri});
            }

            return info.Publishers;
        }


        public bool UnregisterSubscriber(string topic, Uri uri)
        {
            if(_topicRegistrationInfos.ContainsKey(topic))
            {
                var index = _topicRegistrationInfos[topic].Subscribers.FindIndex(x => x.Uri == uri);
                if (index != -1)
                {
                    _topicRegistrationInfos[topic].Subscribers.RemoveAt(index);
                }

                if (_topicRegistrationInfos[topic].Subscribers.Count == 0 && _topicRegistrationInfos[topic].Publishers.Count == 0)
                {
                    _topicRegistrationInfos.Remove(topic);
                }
            }

            return true;
        }

        public TopicRegistrationInfo RegisterPublisher(string nodeId, string topic, string topicType, Uri slaveUri)
        {
            TopicRegistrationInfo info;
            if (_topicRegistrationInfos.ContainsKey(topic))
            {
                info = _topicRegistrationInfos[topic];
            }
            else
            {
                info = new TopicRegistrationInfo()
                {
                    TopicName = topic,
                    TopicType = topicType
                };
                _topicRegistrationInfos.Add(topic, info);
            }

            if (!info.Publishers.Any(x => x.NodeId == nodeId && x.Uri == slaveUri))
            {
                info.Publishers.Add(new RegistrationInfo() { NodeId = nodeId, Uri = slaveUri });
            }

            return info;
        }

        public bool UnregisterPublisher(string topic, Uri uri)
        {
            if (_topicRegistrationInfos.ContainsKey(topic))
            {
                var index = _topicRegistrationInfos[topic].Publishers.FindIndex(x => x.Uri == uri);
                if (index != -1)
                {
                    _topicRegistrationInfos[topic].Publishers.RemoveAt(index);
                }

                if (_topicRegistrationInfos[topic].Subscribers.Count == 0 && _topicRegistrationInfos[topic].Publishers.Count == 0)
                {
                    _topicRegistrationInfos.Remove(topic);
                }
            }

            return true;
        }

        public List<TopicRegistrationInfo> GetPublishers()
        {
            return _topicRegistrationInfos.Values
                .Where(topic => topic.Publishers.Count != 0)
                .ToList();
        }

        public List<TopicRegistrationInfo> GetSubscribers()
        {
            return _topicRegistrationInfos.Values
                .Where(topic => topic.Subscribers.Count != 0)
                .ToList();
        }

        public List<ServiceRegistrationInfo> GetServices()
        {
            return _serviceRegistrationInfos.Values.ToList();
        }
        

        public Uri LookUpNode(string nodeName)
        {
            throw new NotImplementedException();
        }
    }

    internal class RegistrationInfo
    {
        public string NodeId { get; set; }
        public Uri Uri { get; set; }
    }

    internal class TopicRegistrationInfo
    {
        public TopicRegistrationInfo()
        {
            Subscribers = new List<RegistrationInfo>();
            Publishers = new List<RegistrationInfo>();
        }

        public string TopicName { get; set; }
        public string TopicType { get; set; }

        public List<RegistrationInfo> Subscribers { get; private set; }
        public List<RegistrationInfo> Publishers { get; private set; }
    }

    internal class ServiceRegistrationInfo
    {
        public string ServiceName { get; set; }
        public string ServiceType { get; set; }

        //TODO: 同じ名前で複数のサービスを登録できるようにすべき？
        public RegistrationInfo Service { get; set; }
    }
}