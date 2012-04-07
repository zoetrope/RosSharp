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
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Threading.Tasks;
using Common.Logging;
using CookComputing.XmlRpc;
using RosSharp.Topic;
using RosSharp.Transport;

namespace RosSharp.Slave
{
    /// <summary>
    ///   XML-RPC Server for Slave API
    /// </summary>
    public sealed class SlaveServer : MarshalByRefObject, ISlave, IDisposable
    {
        private readonly HttpServerChannel _channel;
        private readonly TcpRosListener _tcpRosListener;
        private readonly TopicContainer _topicContainer;
        private string _nodeId;

        private ILog _logger = LogManager.GetCurrentClassLogger();

        internal SlaveServer(string nodeId, int portNumber, TopicContainer topicContainer, TcpRosListener listener)
        {
            _nodeId = nodeId;
            _topicContainer = topicContainer;
            _tcpRosListener = listener;

            string slaveName = nodeId + "_slave";

            _channel = new HttpServerChannel(slaveName, portNumber, new XmlRpcServerFormatterSinkProvider());
            var tmp = new Uri(_channel.GetChannelUri());

            SlaveUri = new Uri("http://" + ROS.HostName + ":" + tmp.Port + "/" + slaveName);

            ChannelServices.RegisterChannel(_channel, false);
            RemotingServices.Marshal(this, slaveName);
        }

        public Uri SlaveUri { get; private set; }

        #region IDisposable Members

        public void Dispose()
        {
            ChannelServices.UnregisterChannel(_channel);
            RemotingServices.Disconnect(this);
        }

        #endregion

        #region ISlave Members

        /// <summary>
        ///   Retrieve transport/topic statistics.
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <returns>
        /// [0] = int: code <br />
        /// [1] = str: status message <br />
        /// [2] = stats: [publishStats, subscribeStats, serviceStats] <br /> 
        ///   publishStats: [[topicName, messageDataSent, pubConnectionData]...] <br />
        ///   subscribeStats: [[topicName, subConnectionData]...] <br />
        ///   serviceStats: (proposed) [numRequests, bytesReceived, bytesSent] <br />
        ///     pubConnectionData: [connectionId, bytesSent, numSent, connected]* <br />
        ///     subConnectionData: [connectionId, bytesReceived, dropEstimate, connected]*
        /// </returns>
        public object[] GetBusStats(string callerId)
        {
            _logger.Debug(m => m("GetBusStats(callerId={0})", callerId));
            throw new NotImplementedException();
        }

        /// <summary>
        ///   Retrieve transport/topic connection information.
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <returns>
        /// [0] = int: code <br/>
        /// [1] = str: status message<br/>
        /// [2] = businfo: [[connectionId1, destinationId1, direction1, transport1, topic1, connected1]... ] <br/>
        ///   connectionId is defined by the node and is opaque. <br/>
        ///   destinationId is the XMLRPC URI of the destination. <br/>
        ///   direction is one of 'i', 'o', or 'b' (in, out, both). <br/>
        ///   transport is the transport type (e.g. 'TCPROS'). <br/>
        ///   topic is the topic name. <br/>
        ///   connected1 indicates connection status.
        /// </returns>
        public object[] GetBusInfo(string callerId)
        {
            _logger.Debug(m => m("GetBusInfo(callerId={0})", callerId));
            throw new NotImplementedException();
        }

        /// <summary>
        ///   Get the URI of the master node.
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <returns>
        /// [0] = int: code <br/> 
        /// [1] = str: status message <br/>
        /// [2] = str: URI of the master 
        /// </returns>
        public object[] GetMasterUri(string callerId)
        {
            _logger.Debug(m => m("GetMasterUri(callerId={0})", callerId));
            return new object[3]
            {
                StatusCode.Success,
                "",
                ROS.MasterUri //TODO: この実装でよい？
            };
        }

        /// <summary>
        ///   Stop this server.
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <param name="msg"> A message describing why the node is being shutdown. </param>
        /// <returns>
        /// [0] = int: code <br/>
        /// [1] = str: status message <br/>
        /// [2] = int: ignore
        /// </returns>
        public object[] Shutdown(string callerId, string msg)
        {
            _logger.Debug(m => m("Shutdown(callerId={0},msg={1})", callerId, msg));
            throw new NotImplementedException();
        }

        /// <summary>
        ///   Get the PID of this server.
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <returns>
        /// [0] = int: code <br/>
        /// [1] = str: status message <br/>
        /// [2] = int: server process pid
        /// </returns>
        public object[] GetPid(string callerId)
        {
            _logger.Debug(m => m("GetPid(callerId={0})", callerId));
            return new object[3]
            {
                StatusCode.Success,
                "",
                Process.GetCurrentProcess().Id
            };
        }

        /// <summary>
        ///   Retrieve a list of topics that this node subscribes to
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <returns>
        /// [0] = int: code <br/>
        /// [1] = str: status message <br/>
        /// [2] = topicList is a list of topics this node subscribes to and is of the form [ [topic1, topicType1]...[topicN, topicTypeN]]]
        /// </returns>
        public object[] GetSubscriptions(string callerId)
        {
            _logger.Debug(m => m("GetSubscriptions(callerId={0})", callerId));
            return new object[]
            {
                1,
                "Success",
                _topicContainer.GetSubscribers().Select(x => new object[] {x.TopicName, x.MessageType}).ToArray()
            };
        }

        /// <summary>
        ///   Retrieve a list of topics that this node publishes.
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <returns>
        /// [0] = int: code <br/>
        /// [1] = str: status message <br/>
        /// [2] = topicList is a list of topics published by this node and is of the form [ [topic1, topicType1]...[topicN, topicTypeN]]]
        /// </returns>
        public object[] GetPublications(string callerId)
        {
            _logger.Debug(m => m("GetPublications(callerId={0})", callerId));
            return new object[]
            {
                1,
                "Success",
                _topicContainer.GetPublishers().Select(x => new object[] {x.TopicName, x.MessageType}).ToArray()
            };
        }

        /// <summary>
        ///   Callback from master with updated value of subscribed parameter.
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <param name="parameterKey"> Parameter name, globally resolved. </param>
        /// <param name="parameterValue"> New parameter value. </param>
        /// <returns>
        /// [0] = int: code <br/>
        /// [1] = str: status message <br/>
        /// [2] = int: ignore
        /// </returns>
        public object[] ParamUpdate(string callerId, string parameterKey, object parameterValue)
        {
            _logger.Debug(m => m("ParamUpdate(callerId={0},parameterKey={1},parameterValue={2})"
                                 , callerId, parameterKey, parameterValue));

            var handler = ParameterUpdated;
            if (handler != null)
            {
                Task.Factory.FromAsync(handler.BeginInvoke, handler.EndInvoke, parameterKey, parameterValue, null)
                    .ContinueWith(task => _logger.Error("PramUpdateError", task.Exception)
                                  , TaskContinuationOptions.OnlyOnFaulted);
            }

            //TODO:戻り値を調べる
            return new object[]
            {
                StatusCode.Success,
                "parameter update [" + parameterKey + "]",
                0
            };
        }

        /// <summary>
        ///   Callback from master of current publisher list for specified topic.
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <param name="topic"> Topic name. </param>
        /// <param name="publishers"> List of current publishers for topic in the form of XMLRPC URIs </param>
        /// <returns>
        /// [0] = int: code <br/>
        /// [1] = str: status message <br/>
        /// [2] = int: ignore
        /// </returns>
        public object[] PublisherUpdate(string callerId, string topic, string[] publishers)
        {
            _logger.Debug(m => m("PublisherUpdate(callerId={0},topic={1},publishers={2})"
                                 , callerId, topic, publishers));
            if (_topicContainer.HasSubscriber(topic))
            {
                //TODO: TryGet?
                var subs = _topicContainer.GetSubscribers().First(s => s.TopicName == topic);

                //TODO: 非同期に
                subs.UpdatePublishers(publishers.Select(x => new Uri(x)).ToList());
            }

            return new object[3]
            {
                StatusCode.Success,
                "Publisher update received.",
                0
            };
        }

        /// <summary>
        ///   Publisher node API method called by a subscriber node. <br/>
        ///   This requests that source allocate a channel for communication. <br/>
        ///   Subscriber provides a list of desired protocols for communication. <br/>
        ///   Publisher returns the selected protocol along with any additional params required for establishing connection. <br/>
        ///   For example, for a TCP/IP-based connection, the source node may return a port number of TCP/IP server.
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <param name="topic"> Topic name. </param>
        /// <param name="protocols"> List of desired protocols for communication in order of preference. Each protocol is a list of the form [ProtocolName, ProtocolParam1, ProtocolParam2...N] </param>
        /// <returns>
        /// [0] = int: code <br/>
        /// [1] = str: status message <br/>
        /// [2] = protocolParams may be an empty list if there are no compatible protocols. 
        /// </returns>
        public object[] RequestTopic(string callerId, string topic, object[] protocols)
        {
            _logger.Debug(m => m("RequestTopic(callerId={0},topic={1},protocols={2})"
                                 , callerId, topic, protocols));

            if (!_topicContainer.HasPublisher(topic))
            {
                _logger.Warn(m => m("No publishers for topic: ", topic));
                return new object[]
                {
                    -1,
                    "No publishers for topic: " + topic,
                    "null"
                };
            }

            foreach (string[] protocol in protocols)
            {
                string protocolName = protocol[0];

                if (protocolName != "TCPROS") //TODO: ほかのプロトコルにも対応できるように
                {
                    continue;
                }

                var address = _tcpRosListener.EndPoint;

                return new object[3]
                {
                    1,
                    "Protocol<" + protocolName + ", AdvertiseAddress<" + address.ToString() + ">>",
                    new object[3]
                    {
                        protocolName,
                        ROS.HostName,
                        address.Port
                    }
                };
            }

            _logger.Warn("No supported protocols specified.");

            return new object[]
            {
                -1,
                "No supported protocols specified.",
                "null"
            };
        }

        #endregion

        public override object InitializeLifetimeService()
        {
            return null;
        }

        internal event Action<string, object> ParameterUpdated;
    }
}