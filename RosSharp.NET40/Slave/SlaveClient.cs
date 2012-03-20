using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using RosSharp.Topic;

namespace RosSharp.Slave
{
    /// <summary>
    /// XML-RPC Client for Slave API
    /// </summary>
    public sealed class SlaveClient
    {
        private SlaveProxy _proxy;
        public SlaveClient(Uri uri)
        {
            _proxy = new SlaveProxy();
            _proxy.Url = uri.ToString();
        }

        /// <summary>
        /// Retrieve transport/topic statistics.
        /// </summary>
        /// <param name="callerId">ROS caller ID.</param>
        /// <returns>stats</returns>
        public IObservable<object[]> GetBusStatsAsync(string callerId)
        {
            return Observable.FromAsyncPattern<string, object[]>(_proxy.BeginGetBusStats, _proxy.EndGetBusStats)
                .Invoke(callerId)
                .Do(ret => { if ((int)ret[0] != 1) throw new InvalidOperationException((string)ret[1]); });
                
        }

        /// <summary>
        /// Retrieve transport/topic connection information.
        /// </summary>
        /// <param name="callerId">ROS caller ID.</param>
        /// <returns>businfo</returns>
        public IObservable<object[]> GetBusInfoAsync(string callerId)
        {
            return Observable.FromAsyncPattern<string, object[]>(_proxy.BeginGetBusInfo, _proxy.EndGetBusInfo)
                .Invoke(callerId)
                .Do(ret => { if ((int)ret[0] != 1) throw new InvalidOperationException((string)ret[1]); });
        }

        /// <summary>
        /// Get the URI of the master node.
        /// </summary>
        /// <param name="callerId">ROS caller ID.</param>
        /// <returns>URI of the master</returns>
        public IObservable<Uri> GetMasterUriAsync(string callerId)
        {
            return Observable.FromAsyncPattern<string, object[]>(_proxy.BeginGetMasterUri, _proxy.EndGetMasterUri)
                .Invoke(callerId)
                .Do(ret => { if ((int)ret[0] != 1) throw new InvalidOperationException((string)ret[1]); })
                .Select(ret => new Uri((string)ret[2]));
        }

        /// <summary>
        /// Stop this server.
        /// </summary>
        /// <param name="callerId">ROS caller ID.</param>
        /// <param name="msg">A message describing why the node is being shutdown.</param>
        /// <returns>ignore</returns>
        public IObservable<int> ShutdownAsync(string callerId, string msg)
        {
            return Observable.FromAsyncPattern<string, string, object[]>(_proxy.BeginShutdown, _proxy.EndShutdown)
                .Invoke(callerId, msg)
                .Do(ret => { if ((int)ret[0] != 1) throw new InvalidOperationException((string)ret[1]); })
                .Select(ret => (int)ret[2]);
        }

        /// <summary>
        /// Get the PID of this server.
        /// </summary>
        /// <param name="callerId">ROS caller ID.</param>
        /// <returns>server process pid</returns>
        public IObservable<int> GetPidAsync(string callerId)
        {
            return Observable.FromAsyncPattern<string, object[]>(_proxy.BeginGetPid, _proxy.EndGetPid)
                .Invoke(callerId)
                .Do(ret => { if ((int)ret[0] != 1) throw new InvalidOperationException((string)ret[1]); })
                .Select(ret => (int)ret[2]);
        }

        /// <summary>
        /// Retrieve a list of topics that this node subscribes to
        /// </summary>
        /// <param name="callerId">ROS caller ID.</param>
        /// <returns>
        /// topicList is a list of topics this node subscribes to and is of the form
        /// </returns>
        public IObservable<List<TopicInfo>> GetSubscriptionsAsync(string callerId)
        {
            return Observable.FromAsyncPattern<string, object[]>(_proxy.BeginGetSubscriptions,_proxy.EndGetSubscriptions)
                .Invoke(callerId)
                .Do(ret => { if ((int) ret[0] != 1) throw new InvalidOperationException((string) ret[1]); })
                .Select(ret => ((string[][]) ret[2])
                    .Select(x => new TopicInfo() { Name = (string)x[0], Type = (string)x[1] }).ToList());
        }

        /// <summary>
        /// Retrieve a list of topics that this node publishes.
        /// </summary>
        /// <param name="callerId">ROS caller ID.</param>
        /// <returns>
        /// topicList is a list of topics published by this node and is of the form
        /// </returns>
        public IObservable<List<TopicInfo>> GetPublicationsAsync(string callerId)
        {
            return Observable.FromAsyncPattern<string, object[]>(_proxy.BeginGetPublications,_proxy.EndGetPublications)
                .Invoke(callerId)
                .Do(ret => { if ((int)ret[0] != 1) throw new InvalidOperationException((string)ret[1]); })
                .Select(ret => ((object[])ret[2])
                    .Select(x => new TopicInfo() { Name = ((string[])x)[0], Type = ((string[])x)[1] }).ToList());
        }

        /// <summary>
        /// Callback from master with updated value of subscribed parameter.
        /// </summary>
        /// <param name="callerId">ROS caller ID.</param>
        /// <param name="parameterKey">Parameter name, globally resolved.</param>
        /// <param name="parameterValue">New parameter value.</param>
        /// <returns>ignore</returns>
        public IObservable<int> ParamUpdateAsync(string callerId, string parameterKey, object parameterValue)
        {
#if WINDOWS_PHONE
            return ObservableEx
#else
            return Observable
#endif
                .FromAsyncPattern<string, string, object, object[]>(_proxy.BeginParamUpdate, _proxy.EndParamUpdate)
                .Invoke(callerId,parameterKey,parameterValue)
                .Do(ret => { if ((int)ret[0] != 1) throw new InvalidOperationException((string)ret[1]); })
                .Select(ret => (int)ret[2]);
        }

        /// <summary>
        /// Callback from master of current publisher list for specified topic.
        /// </summary>
        /// <param name="callerId">ROS caller ID.</param>
        /// <param name="topic">Topic name.</param>
        /// <param name="publishers">List of current publishers for topic in the form of XMLRPC URIs</param>
        /// <returns>ignore</returns>
        public IObservable<int> PublisherUpdateAsync(string callerId, string topic, string[] publishers)
        {
#if WINDOWS_PHONE
            return ObservableEx
#else
            return Observable
#endif
                .FromAsyncPattern<string, string, string[], object[]>(_proxy.BeginPublisherUpdate, _proxy.EndPublisherUpdate)
                .Invoke(callerId,topic,publishers)
                .Do(ret => { if ((int)ret[0] != 1) throw new InvalidOperationException((string)ret[1]); })
                .Select(ret => (int)ret[2]);
        }

        /// <summary>
        /// Publisher node API method called by a subscriber node.
        /// This requests that source allocate a channel for communication.
        /// Subscriber provides a list of desired protocols for communication.
        /// Publisher returns the selected protocol along with any additional params required for establishing connection.
        /// For example, for a TCP/IP-based connection, the source node may return a port number of TCP/IP server.
        /// </summary>
        /// <param name="callerId">ROS caller ID.</param>
        /// <param name="topic">Topic name.</param>
        /// <param name="protocols">
        /// List of desired protocols for communication in order of preference. Each protocol is a list of the form
        ///   [ProtocolName, ProtocolParam1, ProtocolParam2...N]
        /// </param>
        /// <returns>
        /// protocolParams may be an empty list if there are no compatible protocols.
        /// </returns>
        public IObservable<TopicParam> RequestTopicAsync(string callerId, string topic, object[] protocols)
        {
            //TODO: protocolsの型を明確に。
#if WINDOWS_PHONE
            return ObservableEx
#else
            return Observable
#endif
.FromAsyncPattern<string, string, object[], object[]>(_proxy.BeginRequestTopic, _proxy.EndRequestTopic)
                .Invoke(callerId, topic, protocols)
                .Do(ret => { if ((int)ret[0] != 1) throw new InvalidOperationException((string)ret[1]); })
                .Select(ret => new TopicParam
                {
                    ProtocolName = (string)((object[])ret[2])[0],
                    HostName = (string)((object[])ret[2])[1],
                    PortNumber = (int)((object[])ret[2])[2]
                });
        }
    }

    public class TopicParam
    {
        public string ProtocolName { get; set; }
        public string HostName { get; set; }
        public int PortNumber { get; set; }
    }

    public class TopicInfo : ITopic
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
