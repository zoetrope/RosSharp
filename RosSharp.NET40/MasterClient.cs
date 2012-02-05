using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CookComputing.XmlRpc;
using System.Reactive.Linq;

namespace RosSharp
{
    public class MasterClient
    {
        private readonly MasterProxy _proxy;
        public MasterClient()
        {
            _proxy = new MasterProxy();
            _proxy.Url = "http://192.168.11.4:11311/";
        }


        public object[] RegisterServiceAsync(string callerId, string service, string serviceApi, string callerApi)
        {
            throw new NotImplementedException();
        }

        public object[] UnregisterServiceAsync(string callerId, string service, string serviceApi)
        {
            throw new NotImplementedException();
        }

        public object[] RegisterSubscriberAsync(string callerId, string topic, string topicType, string callerApi)
        {
            throw new NotImplementedException();
        }

        public object[] UnregisterSubscriberAsync(string callerId, string topic, string callerApi)
        {
            throw new NotImplementedException();
        }

        public object[] RegisterPublisherAsync(string callerId, string topic, string topicType, string callerApi)
        {
            throw new NotImplementedException();
        }

        public object[] UnregisterPublisherAsync(string callerId, string topic, string callerApi)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Looking Uri about publishers and subscribers
        /// </summary>
        /// <param name="callerId">ROS Caller ID</param>
        /// <param name="nodeName">Name of node to lookup</param>
        /// <returns>Uri of the node with associated nodeName/callerId</returns>
        public IObservable<Uri> LookupNodeAsync(string callerId, string nodeName)
        {
            return Observable.FromAsyncPattern<string, string, object[]>(_proxy.BeginLookupNode, _proxy.EndLookupNode)
                .Invoke(callerId, nodeName)
                .Do(ret => { if ((int)ret[0] != 1) throw new InvalidOperationException((string)ret[1]); })
                .Select(ret => new Uri((string)ret[2]));

        }

        public object[] GetPublisherTopicsAsync(string callerId, string subgraph)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get system state
        /// </summary>
        /// <param name="callerId">ROS Caller ID</param>
        /// <returns>system state (publishers, subscribers, services)</returns>
        public IObservable<SystemState> GetSystemStateAsync(string callerId)
        {
            return Observable.FromAsyncPattern<string, object[]>(_proxy.BeginGetSystemState, _proxy.EndGetSystemState)
                .Invoke(callerId)
                .Do(ret => { if ((int)ret[0] != 1) throw new InvalidOperationException((string)ret[1]); })
                .Select(ret =>
                    new SystemState()
                    {
                        Publishers = ((object[][][])ret[2])[0]
                        .Select(x => new PublisherSystemState()
                            {
                                TopicName = (string)x[0],
                                Publishers = ((object[])x[1]).Cast<string>().ToList()
                            }).ToList(),
                        Subscribers = ((object[][][])ret[2])[1]
                        .Select(x => new SubscriberSystemState()
                            {
                                TopicName = (string)x[0],
                                Subscribers = ((object[])x[1]).Cast<string>().ToList()
                            }).ToList(),
                        Services = ((object[][][])ret[2])[2]
                        .Select(x => new ServiceSystemState()
                            {
                                ServiceName = (string)x[0],
                                Services = ((object[])x[1]).Cast<string>().ToList()
                            }).ToList()
                    });
        }


        public IObservable<Uri> GetUriAsync(string callerId)
        {
            return Observable.FromAsyncPattern<string, object[]>(_proxy.BeginGetUri, _proxy.EndGetUri)
                .Invoke(callerId)
                .Do(ret => { if ((int)ret[0] != 1) throw new InvalidOperationException((string)ret[1]); })
                .Select(ret => new Uri((string)ret[2]));
        }

        public object[] LookupServiceAsync(string callerId, string service)
        {
            throw new NotImplementedException();
        }
    }

    public class SystemState
    {
        public List<PublisherSystemState> Publishers { get; set; }
        public List<SubscriberSystemState> Subscribers { get; set; }
        public List<ServiceSystemState> Services { get; set; }
    }

    public class PublisherSystemState
    {
        public string TopicName { get; set; }
        public List<string> Publishers { get; set; }
    }
    public class SubscriberSystemState
    {
        public string TopicName { get; set; }
        public List<string> Subscribers { get; set; }
    }
    public class ServiceSystemState
    {
        public string ServiceName { get; set; }
        public List<string> Services { get; set; }
    }
}
