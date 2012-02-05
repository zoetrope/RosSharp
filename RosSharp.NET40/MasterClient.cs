using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using CookComputing.XmlRpc;
using System.Reactive.Linq;

namespace RosSharp
{
    public class MasterClient
    {
        private readonly MasterProxy _proxy;
        public MasterClient(Uri uri)
        {
            _proxy = new MasterProxy();
            _proxy.Url = uri.ToString();
        }

        /// <summary>
        /// Register the Service
        /// </summary>
        /// <param name="callerId">ROS Caller ID</param>
        /// <param name="service">Name of Service</param>
        /// <param name="serviceApi">URI of Service</param>
        /// <param name="callerApi">URI of caller node</param>
        /// <returns>None</returns>
        public IObservable<Unit> RegisterServiceAsync(string callerId, string service, Uri serviceApi, Uri callerApi)
        {
#if WINDOWS_PHONE
            return ObservableEx
#else
            return Observable
#endif
                .FromAsyncPattern<string, string, string, string, object[]>(_proxy.BeginRegisterService, _proxy.EndRegisterService)
                .Invoke(callerId, service, serviceApi.ToString(), callerApi.ToString())
                .Do(ret => { if ((int)ret[0] != 1) throw new InvalidOperationException((string)ret[1]); })
                .Select(_ => Unit.Default);
        }

        /// <summary>
        /// Unregister the Service
        /// </summary>
        /// <param name="callerId">ROS Caller ID</param>
        /// <param name="service">Name of Service</param>
        /// <param name="serviceApi">URI of Service</param>
        /// <returns>Number of Unregistrations</returns>
        public IObservable<int> UnregisterServiceAsync(string callerId, string service, Uri serviceApi)
        {
#if WINDOWS_PHONE
            return ObservableEx
#else
            return Observable
#endif
                .FromAsyncPattern<string, string, string, object[]>(_proxy.BeginUnregisterService, _proxy.EndUnregisterService)
                .Invoke(callerId, service, serviceApi.ToString())
                .Do(ret => { if ((int)ret[0] != 1) throw new InvalidOperationException((string)ret[1]); })
                .Select(ret => (int)ret[2]);
        }

        public IObservable<List<Uri>> RegisterSubscriberAsync(string callerId, string topic, string topicType, Uri callerApi)
        {
#if WINDOWS_PHONE
            return ObservableEx
#else
            return Observable
#endif
                .FromAsyncPattern<string, string, string, string, object[]>(_proxy.BeginRegisterSubscriber, _proxy.EndRegisterSubscriber)
                .Invoke(callerId, topic, topicType, callerApi.ToString())
                .Do(ret => { if ((int)ret[0] != 1) throw new InvalidOperationException((string)ret[1]); })
                .Select(ret => ((object[])ret[2]).Select(x => new Uri((string)x)).ToList());
        }

        public IObservable<int> UnregisterSubscriberAsync(string callerId, string topic, Uri callerApi)
        {
#if WINDOWS_PHONE
            return ObservableEx
#else
            return Observable
#endif
                .FromAsyncPattern<string, string, string, object[]>(_proxy.BeginUnregisterSubscriber, _proxy.EndUnregisterSubscriber)
                .Invoke(callerId, topic, callerApi.ToString())
                .Do(ret => { if ((int)ret[0] != 1) throw new InvalidOperationException((string)ret[1]); })
                .Select(ret => (int)ret[2]);
        }

        public IObservable<List<Uri>> RegisterPublisherAsync(string callerId, string topic, string topicType, Uri callerApi)
        {
#if WINDOWS_PHONE
            return ObservableEx
#else
            return Observable
#endif
                .FromAsyncPattern<string, string, string, string, object[]>(_proxy.BeginRegisterPublisher, _proxy.EndRegisterPublisher)
                .Invoke(callerId, topic, topicType, callerApi.ToString())
                .Do(ret => { if ((int)ret[0] != 1) throw new InvalidOperationException((string)ret[1]); })
                .Select(ret => ((object[])ret[2]).Select(x => new Uri((string)x)).ToList());
        }

        public IObservable<int> UnregisterPublisherAsync(string callerId, string topic, Uri callerApi)
        {
#if WINDOWS_PHONE
            return ObservableEx
#else
            return Observable
#endif
                .FromAsyncPattern<string, string, string, object[]>(_proxy.BeginUnregisterPublisher, _proxy.EndUnregisterPublisher)
                .Invoke(callerId, topic, callerApi.ToString())
                .Do(ret => { if ((int)ret[0] != 1) throw new InvalidOperationException((string)ret[1]); })
                .Select(ret => (int)ret[2]);
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

        public IObservable<List<TopicInfo>> GetPublisherTopicsAsync(string callerId, string subgraph)
        {
            return Observable.FromAsyncPattern<string, string, object[]>(_proxy.BeginGetPublisherTopics, _proxy.EndGetPublisherTopics)
                .Invoke(callerId, subgraph)
                .Do(ret => { if ((int)ret[0] != 1) throw new InvalidOperationException((string)ret[1]); })
                .Select(ret =>
                    ((object[])ret[2]).Select(x => new TopicInfo()
                    {
                        TopicName = (string)((object[])x)[0],
                        TypeName = (string)((object[])x)[1]
                    }).ToList());
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

        /// <summary>
        /// Get URI of the master
        /// </summary>
        /// <param name="callerId">ROS Caller ID</param>
        /// <returns>URI of the master</returns>
        public IObservable<Uri> GetUriAsync(string callerId)
        {
            return Observable.FromAsyncPattern<string, object[]>(_proxy.BeginGetUri, _proxy.EndGetUri)
                .Invoke(callerId)
                .Do(ret => { if ((int)ret[0] != 1) throw new InvalidOperationException((string)ret[1]); })
                .Select(ret => new Uri((string)ret[2]));
        }

        public IObservable<Uri> LookupServiceAsync(string callerId, string service)
        {
            return Observable.FromAsyncPattern<string, string, object[]>(_proxy.BeginLookupService, _proxy.EndLookupService)
                .Invoke(callerId, service)
                .Do(ret => { if ((int)ret[0] != 1) throw new InvalidOperationException((string)ret[1]); })
                .Select(ret => new Uri((string)ret[2]));
        }
    }

    public class TopicInfo
    {
        public string TopicName { get; set; }
        public string TypeName { get; set; }
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
