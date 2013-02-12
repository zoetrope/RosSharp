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
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using RosSharp.Topic;

namespace RosSharp.Slave
{
    /// <summary>
    ///   XML-RPC Client for Slave API
    /// </summary>
    public sealed class SlaveClient
    {
        private readonly SlaveProxy _proxy;
        public Uri SlaveUri { get; private set; }

        public SlaveClient(Uri uri)
        {
            SlaveUri = uri;

            _proxy = new SlaveProxy();
            _proxy.Url = uri.ToString();
            _proxy.Timeout = Ros.XmlRpcTimeout;
        }

        /// <summary>
        ///   Retrieve transport/topic statistics.
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <returns> stats </returns>
        public async Task<BusStatistics> GetBusStatsAsync(string callerId)
        {
            var result = await Task<object[]>.Factory.FromAsync(_proxy.BeginGetBusStats, _proxy.EndGetBusStats, callerId, null);
            if ((StatusCode)result[0] != StatusCode.Success) throw new InvalidOperationException((string)result[1]);

            var statistics = (object[])result[2];

            var ret = new BusStatistics();

            if (statistics[0] is object[])
            {
                ret.PublishStatistics = ((object[])statistics[0])
                    .Cast<object[]>()
                    .Select(x => new PublishStatistic()
                    {
                        TopicName = (string)x[0],
                        MessageDataSent = (int)x[1],
                        ConnectionData = ((object[])x[2])
                            .Cast<object[]>()
                            .Select(y => new PublishConnectionData()
                            {
                                ConnectionId = (int)y[0],
                                BytesSent = (int)y[1],
                                NumSent = (int)y[2],
                                Connected = (bool)y[3]
                            })
                            .ToList(),
                    }).ToList();
            }
            if (statistics[1] is object[])
            {
                ret.SubscribeStatistics = ((object[])statistics[1])
                    .Cast<object[]>()
                    .Select(x => new SubscirbeStatistic()
                    {
                        TopicName = (string)x[0],
                        ConnectionData = ((object[])x[1])
                            .Cast<object[]>()
                            .Select(y => new SubscribeConnectionData()
                            {
                                ConnectionId = (int)y[0],
                                BytesReceived = (int)y[1],
                                NumSent = (int)y[2],
                                DropEstimate = (int)y[3],
                                Connected = (bool)y[4]
                            })
                            .ToList(),
                    }).ToList();
            }
            if (statistics[2] is object[])
            {
                //proposed...
            }
            return ret;
        }

        /// <summary>
        ///   Retrieve transport/topic connection information.
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <returns> businfo </returns>
        public async Task<BusInformation> GetBusInfoAsync(string callerId)
        {
            var result = await Task<object[]>.Factory.FromAsync(_proxy.BeginGetBusInfo, _proxy.EndGetBusInfo, callerId, null);
            if ((StatusCode)result[0] != StatusCode.Success) throw new InvalidOperationException((string)result[1]);

            var info = (object[])result[2];
            return new BusInformation()
            {
                ConnectionId = (int)info[0],
                DestinationId = (string)info[1],
                Direction = (string)info[2],
                Transport = (string)info[3],
                Topic = (string)info[4],
                Connected = (bool)info[5]
            };
        }

        /// <summary>
        ///   Get the URI of the master node.
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <returns> URI of the master </returns>
        public async Task<Uri> GetMasterUriAsync(string callerId)
        {
            var result = await Task<object[]>.Factory.FromAsync(_proxy.BeginGetMasterUri, _proxy.EndGetMasterUri, callerId, null);
            if ((StatusCode)result[0] != StatusCode.Success) throw new InvalidOperationException((string)result[1]);
            return new Uri((string)result[2]);
        }

        /// <summary>
        ///   Stop this server.
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <param name="msg"> A message describing why the node is being shutdown. </param>
        /// <returns> ignore </returns>
        public async Task ShutdownAsync(string callerId, string msg)
        {
            var result = await Task<object[]>.Factory.FromAsync(_proxy.BeginShutdown, _proxy.EndShutdown, callerId, msg, null);
            if ((StatusCode)result[0] != StatusCode.Success) throw new InvalidOperationException((string)result[1]);
        }

        /// <summary>
        ///   Get the PID of this server.
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <returns> server process pid </returns>
        public async Task<int> GetPidAsync(string callerId)
        {
            var result = await Task<object[]>.Factory.FromAsync(_proxy.BeginGetPid, _proxy.EndGetPid, callerId, null);
            if ((StatusCode)result[0] != StatusCode.Success) throw new InvalidOperationException((string)result[1]);
            return (int)result[2];
        }

        /// <summary>
        ///   Retrieve a list of topics that this node subscribes to
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <returns> topicList is a list of topics this node subscribes to and is of the form </returns>
        public async Task<List<TopicInfo>> GetSubscriptionsAsync(string callerId)
        {
            var result = await Task<object[]>.Factory.FromAsync(_proxy.BeginGetSubscriptions, _proxy.EndGetSubscriptions, callerId, null);
            if ((StatusCode)result[0] != StatusCode.Success) throw new InvalidOperationException((string)result[1]);
            return ((object[])result[2])
                .Select(x => new TopicInfo() { TopicName = ((string[])x)[0], MessageType = ((string[])x)[1] }).ToList();
        }

        /// <summary>
        ///   Retrieve a list of topics that this node publishes.
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <returns> topicList is a list of topics published by this node and is of the form </returns>
        public async Task<List<TopicInfo>> GetPublicationsAsync(string callerId)
        {
            var result = await Task<object[]>.Factory.FromAsync(_proxy.BeginGetPublications, _proxy.EndGetPublications, callerId, null);
            if ((StatusCode)result[0] != StatusCode.Success) throw new InvalidOperationException((string)result[1]);
            return ((object[])result[2])
                .Select(x => new TopicInfo() { TopicName = ((string[])x)[0], MessageType = ((string[])x)[1] }).ToList();
        }

        /// <summary>
        ///   Callback from master with updated value of subscribed parameter.
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <param name="parameterKey"> Parameter name, globally resolved. </param>
        /// <param name="parameterValue"> New parameter value. </param>
        /// <returns> ignore </returns>
        public async Task ParamUpdateAsync(string callerId, string parameterKey, object parameterValue)
        {
            var result = await Task<object[]>.Factory.FromAsync(_proxy.BeginParamUpdate, _proxy.EndParamUpdate, callerId, parameterKey, parameterValue, null);
            if ((StatusCode)result[0] != StatusCode.Success) throw new InvalidOperationException((string)result[1]);
        }

        /// <summary>
        ///   Callback from master of current publisher list for specified topic.
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <param name="topic"> Topic name. </param>
        /// <param name="publishers"> List of current publishers for topic in the form of XMLRPC URIs </param>
        /// <returns> ignore </returns>
        public async Task PublisherUpdateAsync(string callerId, string topic, string[] publishers)
        {
            var result = await Task<object[]>.Factory.FromAsync(_proxy.BeginPublisherUpdate, _proxy.EndPublisherUpdate, callerId, topic, publishers, null);
            if ((StatusCode)result[0] != StatusCode.Success) throw new InvalidOperationException((string)result[1]);
        }

        /// <summary>
        ///   Publisher node API method called by a subscriber node. This requests that source allocate a channel for communication. Subscriber provides a list of desired protocols for communication. Publisher returns the selected protocol along with any additional params required for establishing connection. For example, for a TCP/IP-based connection, the source node may return a port number of TCP/IP server.
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <param name="topic"> Topic name. </param>
        /// <param name="infos"> List of desired protocols for communication in order of preference. Each protocol is a list of the form [ProtocolName, ProtocolParam1, ProtocolParam2...N] </param>
        /// <returns> protocolParams may be an empty list if there are no compatible protocols. </returns>
        public async Task<TopicParam> RequestTopicAsync(string callerId, string topic, List<ProtocolInfo> infos)
        {
            var protocols = infos.Select(
                info => new List<string>() { info.Protocol.ToString() }.Concat(info.ProtocolParams).ToArray()).ToArray();

            var result = await Task<object[]>.Factory.FromAsync(_proxy.BeginRequestTopic, _proxy.EndRequestTopic, callerId, topic, protocols, null);
            if ((StatusCode)result[0] != StatusCode.Success) throw new InvalidOperationException((string)result[1]);
            return new TopicParam
            {
                ProtocolName = (string)((object[])result[2])[0],
                HostName = (string)((object[])result[2])[1],
                PortNumber = (int)((object[])result[2])[2]
            };
        }
    }

    public sealed class TopicParam
    {
        public string ProtocolName { get; set; }
        public string HostName { get; set; }
        public int PortNumber { get; set; }
    }

    public sealed class TopicInfo : ITopic
    {
        #region ITopic Members

        public string TopicName { get; set; }
        public string MessageType { get; set; }

        #endregion
    }


    public enum ProtocolType
    {
        TCPROS,
        UDPROS
    }
    public sealed class ProtocolInfo
    {
        public ProtocolInfo(ProtocolType type, params string[] parameters)
        {
            Protocol = type;
            ProtocolParams = parameters.ToList();
        }

        public ProtocolType Protocol { get; private set; }
        public List<string> ProtocolParams { get; private set; }
    }

    public sealed class BusStatistics
    {
        public List<PublishStatistic> PublishStatistics { get; set; }
        public List<SubscirbeStatistic> SubscribeStatistics { get; set; }
        public List<ServiceStatistic> ServiceStatistics { get; set; }

        public BusStatistics()
        {
            PublishStatistics = new List<PublishStatistic>();
            SubscribeStatistics = new List<SubscirbeStatistic>();
            ServiceStatistics = new List<ServiceStatistic>();
        }
    }

    public sealed class PublishStatistic
    {
        public PublishStatistic()
        {
            ConnectionData = new List<PublishConnectionData>();
        }
        public string TopicName { get; set; }
        public int MessageDataSent { get; set; }
        public List<PublishConnectionData> ConnectionData { get; set; }
    }

    public sealed class SubscirbeStatistic
    {
        public SubscirbeStatistic()
        {
            ConnectionData = new List<SubscribeConnectionData>();
        }
        public string TopicName { get; set; }
        public List<SubscribeConnectionData> ConnectionData { get; set; }
    }
    
    public sealed class ServiceStatistic
    {
        public int NumRequests { get; set; }
        public int BytesReceived { get; set; }
        public int BytesSent { get; set; }
    }

    public sealed class PublishConnectionData
    {
        public int ConnectionId { get; set; }
        public int BytesSent { get; set; }
        public int NumSent { get; set; }
        public bool Connected { get; set; }
    }
    
    public sealed class SubscribeConnectionData
    {
        public int ConnectionId { get; set; }
        public int BytesReceived { get; set; }
        public int NumSent { get; set; }
        public int DropEstimate { get; set; }
        public bool Connected { get; set; }
    }

    public sealed class BusInformation
    {
        public int ConnectionId { get; set; }
        public string DestinationId { get; set; }
        public string Direction { get; set; }
        public string Transport { get; set; }
        public string Topic { get; set; }
        public bool Connected { get; set; }
    }

}
