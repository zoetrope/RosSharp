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

namespace RosSharp.Master
{
    /// <summary>
    ///   Defines interface for Master API
    /// </summary>
    /// <remarks>
    ///   http://www.ros.org/wiki/ROS/Master_API
    /// </remarks>
    [XmlRpcUrl("")]
    internal interface IMaster
    {
        /// <summary>
        ///   Register the caller as a provider of the specified service.
        /// </summary>
        /// <param name="callerId"> ROS Caller ID </param>
        /// <param name="service"> Fully-qualified name of service </param>
        /// <param name="serviceApi"> ROSRPC Service URI </param>
        /// <param name="callerApi"> XML-RPC URI of caller node </param>
        /// <returns>
        /// [0] = int: code <br/>
        /// [1] = str: status message <br/>
        /// [2] = int: ignore
        /// </returns>
        [XmlRpcMethod("registerService")]
        object[] RegisterService(string callerId, string service, string serviceApi, string callerApi);

        /// <summary>
        ///   Unregister the caller as a provider of the specified service.
        /// </summary>
        /// <param name="callerId"> ROS Caller ID </param>
        /// <param name="service"> Fully-qualified name of service </param>
        /// <param name="serviceApi"> API URI of service to unregister. Unregistration will only occur if current registration matches. </param>
        /// <returns>
        /// [0] = int: code <br/>
        /// [1] = str: status message <br/>
        /// [2] = int: Number of Unregistrations
        /// </returns>
        [XmlRpcMethod("unregisterService")]
        object[] UnregisterService(string callerId, string service, string serviceApi);

        /// <summary>
        ///   Subscribe the caller to the specified topic. <br/>
        ///   In addition to receiving a list of current publishers, the subscriber will also receive notifications of new publishers via the publisherUpdate API.
        /// </summary>
        /// <param name="callerId"> ROS caller ID </param>
        /// <param name="topic"> Fully-qualified name of topic. </param>
        /// <param name="topicType"> Datatype for topic. Must be a package-resource name, i.e. the .msg name. </param>
        /// <param name="callerApi"> API URI of subscriber to register. Will be used for new publisher notifications. </param>
        /// <returns>
        /// [0] = int: code <br/>
        /// [1] = str: status message <br/>
        /// [2] = str[]: list of XMLRPC API URIs for nodes currently publishing the specified topic. 
        /// </returns>
        [XmlRpcMethod("registerSubscriber")]
        object[] RegisterSubscriber(string callerId, string topic, string topicType, string callerApi);

        /// <summary>
        ///   Unregister the caller as a publisher of the topic.
        /// </summary>
        /// <param name="callerId"> ROS caller ID </param>
        /// <param name="topic"> Fully-qualified name of topic. </param>
        /// <param name="callerApi"> API URI of service to unregister. Unregistration will only occur if current registration matches. </param>
        /// <returns>
        /// [0] = int: code <br/>
        /// [1] = str: status message <br/>
        /// [2] = int: Number of Unsubscribed
        /// </returns>
        [XmlRpcMethod("unregisterSubscriber")]
        object[] UnregisterSubscriber(string callerId, string topic, string callerApi);

        /// <summary>
        ///   Register the caller as a publisher the topic.
        /// </summary>
        /// <param name="callerId"> ROS caller ID </param>
        /// <param name="topic"> Fully-qualified name of topic to register. </param>
        /// <param name="topicType"> Datatype for topic. Must be a package-resource name, i.e. the .msg name. </param>
        /// <param name="callerApi"> API URI of publisher to register. </param>
        /// <returns>
        /// [0] = int: code <br/>
        /// [1] = str: status message <br/>
        /// [2] = str[]: List of current subscribers of topic in the form of XMLRPC URIs.
        /// </returns>
        [XmlRpcMethod("registerPublisher")]
        object[] RegisterPublisher(string callerId, string topic, string topicType, string callerApi);

        /// <summary>
        ///   Unregister the caller as a publisher of the topic.
        /// </summary>
        /// <param name="callerId"> ROS caller ID </param>
        /// <param name="topic"> Fully-qualified name of topic to unregister. </param>
        /// <param name="callerApi"> API URI of publisher to unregister. Unregistration will only occur if current registration matches. </param>
        /// <returns>
        /// [0] = int: code <br/>
        /// [1] = str: status message <br/>
        /// [2] = int: Number of Unregistered
        /// </returns>
        [XmlRpcMethod("unregisterPublisher")]
        object[] UnregisterPublisher(string callerId, string topic, string callerApi);

        /// <summary>
        ///   Get the XML-RPC URI of the node with the associated name/caller_id. <br/>
        ///   This API is for looking information about publishers and subscribers. Use lookupService instead to lookup ROS-RPC URIs.
        /// </summary>
        /// <param name="callerId"> ROS Caller ID </param>
        /// <param name="nodeName"> Name of node to lookup </param>
        /// <returns>
        /// [0] = int: code <br/>
        /// [1] = str: status message <br/>
        /// [2] = str: URI of the Node
        /// </returns>
        [XmlRpcMethod("lookupNode")]
        object[] LookupNode(string callerId, string nodeName);

        /// <summary>
        ///   Get list of topics that can be subscribed to. This does not return topics that have no publishers. See getSystemState() to get more comprehensive list.
        /// </summary>
        /// <param name="callerId"> ROS caller ID </param>
        /// <param name="subgraph"> Restrict topic names to match within the specified subgraph. Subgraph namespace is resolved relative to the caller's namespace. Use emptry string to specify all names. </param>
        /// <returns>
        /// [0] = int: code
        /// [1] = str: status message
        /// [2] = str[][]: [topic1, type1]...[topicN, typeN]
        /// </returns>
        [XmlRpcMethod("getPublisherTopics")]
        object[] GetPublisherTopics(string callerId, string subgraph);

        /// <summary>
        ///   Retrieve list representation of system state (i.e. publishers, subscribers, and services).
        /// </summary>
        /// <param name="callerId"> ROS Caller ID </param>
        /// <returns>
        /// [0] = int: code
        /// [1] = str: status message
        /// [2] = System state is in list representation [publishers, subscribers, services] <br/>
        ///   publishers: [ [topic1, [topic1Publisher1...topic1PublisherN]] ... ] <br/>
        ///   subscribers: [ [topic1, [topic1Subscriber1...topic1SubscriberN]] ... ] <br/>
        ///   services: [ [service1, [service1Provider1...service1ProviderN]] ... ]
        /// </returns>
        [XmlRpcMethod("getSystemState")]
        object[] GetSystemState(string callerId);

        /// <summary>
        ///   Get the URI of the the master.
        /// </summary>
        /// <param name="callerId"> ROS Caller ID </param>
        /// <returns>
        /// [0] = int: code <br/>
        /// [1] = str: status message <br/>
        /// [2] = str: URI of the master
        /// </returns>
        [XmlRpcMethod("getUri")]
        object[] GetUri(string callerId);

        /// <summary>
        ///   Lookup all provider of a particular service.
        /// </summary>
        /// <param name="callerId"> ROS caller ID </param>
        /// <param name="service"> Fully-qualified name of service </param>
        /// <returns>
        /// [0] = int: code <br/>
        /// [1] = str: status message <br/>
        /// [2] = str: URI of the service
        /// </returns>
        [XmlRpcMethod("lookupService")]
        object[] LookupService(string callerId, string service);

        ///////////////////////////////////////////////////////////////
        // External API
        ///////////////////////////////////////////////////////////////

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
    }
}