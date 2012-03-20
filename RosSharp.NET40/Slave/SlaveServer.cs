using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using CookComputing.XmlRpc;
using RosSharp.Topic;
using RosSharp.Transport;

namespace RosSharp.Slave
{
    /// <summary>
    /// XML-RPC Server for Slave API
    /// </summary>
    public sealed class SlaveServer : MarshalByRefObject, ISlave, IDisposable
    {
        //TODO: サーバ実装を委譲してinternalクラスにしたほうがよいか。

        private readonly TopicContainer _topicContainer;
        private readonly RosTopicServer _rosTopicServer;

        public Uri SlaveUri { get; set; }

        public SlaveServer(int portNumber, TopicContainer topicContainer, RosTopicServer topicServer)
        {
            _topicContainer = topicContainer;
            _rosTopicServer = topicServer;

            var channel = new HttpServerChannel("slave", portNumber, new XmlRpcServerFormatterSinkProvider());
            var tmp = new Uri(channel.GetChannelUri());

            SlaveUri = new Uri("http://" + ROS.LocalHostName + ":" + tmp.Port + "/slave");

            ChannelServices.RegisterChannel(channel, false);
            RemotingServices.Marshal(this, "slave");
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        /// <summary>
        /// Retrieve transport/topic statistics.
        /// </summary>
        /// <param name="callerId">ROS caller ID.</param>
        /// <returns>
        /// int: code
        /// str: status message
        /// stats:
        ///   [publishStats, subscribeStats, serviceStats]
        ///     publishStats: [[topicName, messageDataSent, pubConnectionData]...]
        ///     subscribeStats: [[topicName, subConnectionData]...]
        ///     serviceStats: (proposed) [numRequests, bytesReceived, bytesSent]
        ///     pubConnectionData: [connectionId, bytesSent, numSent, connected]* 
        ///     subConnectionData: [connectionId, bytesReceived, dropEstimate, connected]*
        /// </returns>
        public object[] GetBusStats(string callerId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieve transport/topic connection information.
        /// </summary>
        /// <param name="callerId">ROS caller ID.</param>
        /// <returns>
        /// int: code
        /// str: status message
        /// businfo:
        ///   [[connectionId1, destinationId1, direction1, transport1, topic1, connected1]... ]
        ///     connectionId is defined by the node and is opaque.
        ///     destinationId is the XMLRPC URI of the destination.
        ///     direction is one of 'i', 'o', or 'b' (in, out, both).
        ///     transport is the transport type (e.g. 'TCPROS').
        ///     topic is the topic name.
        ///     connected1 indicates connection status. 
        /// </returns>
        public object[] GetBusInfo(string callerId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the URI of the master node.
        /// </summary>
        /// <param name="callerId">ROS caller ID.</param>
        /// <returns>
        /// int: code
        /// str: status message
        /// str: URI of the master
        /// </returns>
        public object[] GetMasterUri(string callerId)
        {
            return new object[3]
            {
                1,
                "",
                ROS.MasterUri //TODO: この実装でよい？
            };
        }

        /// <summary>
        /// Stop this server.
        /// </summary>
        /// <param name="callerId">ROS caller ID.</param>
        /// <param name="msg">A message describing why the node is being shutdown.</param>
        /// <returns>
        /// int: code
        /// str: status message
        /// int: ignore
        /// </returns>
        public object[] Shutdown(string callerId, string msg)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the PID of this server.
        /// </summary>
        /// <param name="callerId">ROS caller ID.</param>
        /// <returns>
        /// int: code
        /// str: status message
        /// int: server process pid
        /// </returns>
        public object[] GetPid(string callerId)
        {
            return new object[3]
            {
                1,
                "",
                Process.GetCurrentProcess().Id
            };            
        }

        /// <summary>
        /// Retrieve a list of topics that this node subscribes to
        /// </summary>
        /// <param name="callerId">ROS caller ID.</param>
        /// <returns>
        /// int: code
        /// str: status message
        /// topicList is a list of topics this node subscribes to and is of the form
        ///   [ [topic1, topicType1]...[topicN, topicTypeN]]]
        /// </returns>
        public object[] GetSubscriptions(string callerId)
        {
            return new object[]
            {
                1,
                "Success",
                _topicContainer.GetSubscribers().Select(x => new object[] {x.Name, x.Type}).ToArray()
            };
        }

        /// <summary>
        /// Retrieve a list of topics that this node publishes.
        /// </summary>
        /// <param name="callerId">ROS caller ID.</param>
        /// <returns>
        /// int: code
        /// str: status message
        /// topicList is a list of topics published by this node and is of the form
        ///   [ [topic1, topicType1]...[topicN, topicTypeN]]]
        /// </returns>
        public object[] GetPublications(string callerId)
        {
            return new object[]
            {
                1,
                "Success",
                _topicContainer.GetPublishers().Select(x => new object[] {x.Name, x.Type}).ToArray()
            };
        }

        /// <summary>
        /// Callback from master with updated value of subscribed parameter.
        /// </summary>
        /// <param name="callerId">ROS caller ID.</param>
        /// <param name="parameterKey">Parameter name, globally resolved.</param>
        /// <param name="parameterValue">New parameter value.</param>
        /// <returns>
        /// int: code
        /// str: status message
        /// int: ignore
        /// </returns>
        public object[] ParamUpdate(string callerId, string parameterKey, object parameterValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Callback from master of current publisher list for specified topic.
        /// </summary>
        /// <param name="callerId">ROS caller ID.</param>
        /// <param name="topic">Topic name.</param>
        /// <param name="publishers">List of current publishers for topic in the form of XMLRPC URIs</param>
        /// <returns>
        /// int: code
        /// str: status message
        /// int: ignore
        /// </returns>
        public object[] PublisherUpdate(string callerId, string topic, string[] publishers)
        {
            if(_topicContainer.HasSubscriber(topic))
            {
                var subs = _topicContainer.GetSubscribers().First(s => s.Name == topic);

                //TODO: publishersを渡す。
                subs.UpdatePublishers();
            }

            //TODO: 戻り値は？
            return new object[0];
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
        /// int: code
        /// str: status message
        /// protocolParams may be an empty list if there are no compatible protocols.
        /// </returns>
        public object[] RequestTopic(string callerId, string topic, object[] protocols)
        {
            if(!_topicContainer.HasPublisher(topic))
            {
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

                var address = _rosTopicServer.AdvertiseAddress;
                
                return new object[3]
                {
                    1,
                    "Protocol<" + protocolName + ", AdvertiseAddress<" + address.ToString() + ">>",
                    new object[3]
                    {
                        protocolName,
                        ROS.LocalHostName,
                        address.Port
                    }
                };
            }

            return new object[]
            {
                -1,
                "No supported protocols specified.",
                "null"
            };

        }
    }
}
