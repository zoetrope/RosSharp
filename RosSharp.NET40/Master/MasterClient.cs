using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace RosSharp.Master
{
    /// <summary>
    /// XML-RPC Client for Master API
    /// </summary>
    public sealed class MasterClient
    {
        private readonly MasterProxy _proxy;
        public MasterClient(Uri uri)
        {
            _proxy = new MasterProxy();
            _proxy.Url = uri.ToString();
            _proxy.Timeout = ROS.XmlRpcTimeout;
        }

        /// <summary>
        /// Register the caller as a provider of the specified service.
        /// </summary>
        /// <param name="callerId">ROS Caller ID</param>
        /// <param name="service">Fully-qualified name of service</param>
        /// <param name="serviceApi">ROSRPC Service URI</param>
        /// <param name="callerApi">XML-RPC URI of caller node</param>
        /// <returns>ignore</returns>
        public IObservable<Unit> RegisterServiceAsync(string callerId, string service, Uri serviceApi, Uri callerApi)
        {
#if WINDOWS_PHONE
            return ObservableEx
#else
            return Observable
#endif
                .FromAsyncPattern<string, string, string, string, object[]>(_proxy.BeginRegisterService, _proxy.EndRegisterService)
                .Invoke(callerId, service, serviceApi.ToString(), callerApi.ToString())
                .Do(ret => { if ((StatusCode)ret[0] != StatusCode.Success) throw new InvalidOperationException((string)ret[1]); })
                .Select(_ => Unit.Default);
        }

        /// <summary>
        /// Unregister the caller as a provider of the specified service.
        /// </summary>
        /// <param name="callerId">ROS Caller ID</param>
        /// <param name="service">Fully-qualified name of service</param>
        /// <param name="serviceApi">API URI of service to unregister.
        /// Unregistration will only occur if current registration matches.</param>
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

        /// <summary>
        /// Subscribe the caller to the specified topic.
        /// In addition to receiving a list of current publishers, 
        /// the subscriber will also receive notifications of new publishers via the publisherUpdate API.
        /// </summary>
        /// <param name="callerId">ROS caller ID</param>
        /// <param name="topic">Fully-qualified name of topic.</param>
        /// <param name="topicType">Datatype for topic. Must be a package-resource name, i.e. the .msg name.</param>
        /// <param name="callerApi">API URI of subscriber to register. Will be used for new publisher notifications.</param>
        /// <returns>list of XMLRPC API URIs for nodes currently publishing the specified topic.</returns>
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

        /// <summary>
        /// Unregister the caller as a publisher of the topic.
        /// </summary>
        /// <param name="callerId">ROS caller ID</param>
        /// <param name="topic">Fully-qualified name of topic.</param>
        /// <param name="callerApi">API URI of service to unregister.
        /// Unregistration will only occur if current registration matches.</param>
        /// <returns>Number of Unsubscribed</returns>
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

        /// <summary>
        /// Register the caller as a publisher the topic.
        /// </summary>
        /// <param name="callerId">ROS caller ID</param>
        /// <param name="topic">Fully-qualified name of topic to register.</param>
        /// <param name="topicType">Datatype for topic. Must be a package-resource name, i.e. the .msg name.</param>
        /// <param name="callerApi">API URI of publisher to register.</param>
        /// <returns>List of current subscribers of topic in the form of XMLRPC URIs.</returns>
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

        /// <summary>
        /// Unregister the caller as a publisher of the topic.
        /// </summary>
        /// <param name="callerId">ROS caller ID</param>
        /// <param name="topic">Fully-qualified name of topic to unregister.</param>
        /// <param name="callerApi">API URI of publisher to unregister.
        /// Unregistration will only occur if current registration matches.</param>
        /// <returns>Number of Unregistered</returns>
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
        /// Get the XML-RPC URI of the node with the associated name/caller_id.
        /// This API is for looking information about publishers and subscribers.
        /// Use lookupService instead to lookup ROS-RPC URIs.
        /// </summary>
        /// <param name="callerId">ROS Caller ID</param>
        /// <param name="nodeName">Name of node to lookup</param>
        /// <returns>URI of the Node</returns>
        public IObservable<Uri> LookupNodeAsync(string callerId, string nodeName)
        {
            return Observable.FromAsyncPattern<string, string, object[]>(_proxy.BeginLookupNode, _proxy.EndLookupNode)
                .Invoke(callerId, nodeName)
                .Do(ret => { if ((int)ret[0] != 1) throw new InvalidOperationException((string)ret[1]); })
                .Select(ret => new Uri((string)ret[2]));

        }

        /// <summary>
        /// Get list of topics that can be subscribed to.
        /// This does not return topics that have no publishers.
        /// See getSystemState() to get more comprehensive list.
        /// </summary>
        /// <param name="callerId">ROS caller ID</param>
        /// <param name="subgraph">Restrict topic names to match within the specified subgraph.
        /// Subgraph namespace is resolved relative to the caller's namespace.
        /// Use emptry string to specify all names.</param>
        /// <returns>TopicInfo list</returns>
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
        /// Retrieve list representation of system state (i.e. publishers, subscribers, and services).
        /// </summary>
        /// <param name="callerId">ROS Caller ID</param>
        /// <returns>System state</returns>
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
        /// Get the URI of the the master.
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

        /// <summary>
        /// Lookup all provider of a particular service.
        /// </summary>
        /// <param name="callerId">ROS caller ID</param>
        /// <param name="service">Fully-qualified name of service</param>
        /// <returns>URI of the service</returns>
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
