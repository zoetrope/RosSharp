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

using CookComputing.XmlRpc;

namespace RosSharp.Slave
{
    /// <summary>
    ///   Defines interface for Slave API
    /// </summary>
    /// <remarks>
    ///   http://www.ros.org/wiki/ROS/Slave_API
    /// </remarks>
    [XmlRpcUrl("")]
    internal interface ISlave
    {
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
        ///     subConnectionData: [connectionId, bytesReceived, numSent, dropEstimate, connected]*
        /// </returns>
        [XmlRpcMethod("getBusStats")]
        object[] GetBusStats(string callerId);

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
        [XmlRpcMethod("getBusInfo")]
        object[] GetBusInfo(string callerId);

        /// <summary>
        ///   Get the URI of the master node.
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <returns>
        /// [0] = int: code <br/> 
        /// [1] = str: status message <br/>
        /// [2] = str: URI of the master 
        /// </returns>
        [XmlRpcMethod("getMasterUri")]
        object[] GetMasterUri(string callerId);

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
        [XmlRpcMethod("shutdown")]
        object[] Shutdown(string callerId, string msg);

        /// <summary>
        ///   Get the PID of this server.
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <returns>
        /// [0] = int: code <br/>
        /// [1] = str: status message <br/>
        /// [2] = int: server process pid
        /// </returns>
        [XmlRpcMethod("getPid")]
        object[] GetPid(string callerId);

        /// <summary>
        ///   Retrieve a list of topics that this node subscribes to
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <returns>
        /// [0] = int: code <br/>
        /// [1] = str: status message <br/>
        /// [2] = topicList is a list of topics this node subscribes to and is of the form [ [topic1, topicType1]...[topicN, topicTypeN]]]
        /// </returns>
        [XmlRpcMethod("getSubscriptions")]
        object[] GetSubscriptions(string callerId);

        /// <summary>
        ///   Retrieve a list of topics that this node publishes.
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <returns>
        /// [0] = int: code <br/>
        /// [1] = str: status message <br/>
        /// [2] = topicList is a list of topics published by this node and is of the form [ [topic1, topicType1]...[topicN, topicTypeN]]]
        /// </returns>
        [XmlRpcMethod("getPublications")]
        object[] GetPublications(string callerId);

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
        [XmlRpcMethod("paramUpdate")]
        object[] ParamUpdate(string callerId, string parameterKey, object parameterValue);

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
        [XmlRpcMethod("publisherUpdate")]
        object[] PublisherUpdate(string callerId, string topic, string[] publishers);

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
        [XmlRpcMethod("requestTopic")]
        object[] RequestTopic(string callerId, string topic, object[] protocols);
    }
}