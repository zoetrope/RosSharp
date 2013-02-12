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

using RosSharp.Slave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RosSharp.Master
{
    /// <summary>
    ///   XML-RPC Client for Master API
    /// </summary>
    public sealed class MasterClient
    {
        private readonly MasterProxy _proxy;

        public MasterClient(Uri uri)
        {
            _proxy = new MasterProxy();
            _proxy.Url = uri.ToString();
            _proxy.Timeout = Ros.XmlRpcTimeout;
        }

        /// <summary>
        ///   Register the caller as a provider of the specified service.
        /// </summary>
        /// <param name="callerId"> ROS Caller ID </param>
        /// <param name="service"> Fully-qualified name of service </param>
        /// <param name="serviceApi"> ROSRPC Service URI </param>
        /// <param name="callerApi"> XML-RPC URI of caller node </param>
        /// <returns> ignore </returns>
        public async Task RegisterServiceAsync(string callerId, string service, Uri serviceApi, Uri callerApi)
        {
            var result = await Task<object[]>.Factory.FromAsync((callback, o) =>
                                                    _proxy.BeginRegisterService(callerId, service, serviceApi.ToString(), callerApi.ToString(), callback, o),
                                                    _proxy.EndRegisterService, null);

            if ((StatusCode)result[0] != StatusCode.Success) throw new InvalidOperationException((string)result[1]);

        }

        /// <summary>
        ///   Unregister the caller as a provider of the specified service.
        /// </summary>
        /// <param name="callerId"> ROS Caller ID </param>
        /// <param name="service"> Fully-qualified name of service </param>
        /// <param name="serviceApi"> API URI of service to unregister. Unregistration will only occur if current registration matches. </param>
        /// <returns> Number of Unregistrations </returns>
        public async Task<int> UnregisterServiceAsync(string callerId, string service, Uri serviceApi)
        {
            var result = await Task<object[]>.Factory.FromAsync(_proxy.BeginUnregisterService, _proxy.EndUnregisterService, callerId, service, serviceApi.ToString(), null);
            if ((StatusCode)result[0] != StatusCode.Success) throw new InvalidOperationException((string)result[1]);
            return (int)result[2];
        }

        /// <summary>
        ///   Subscribe the caller to the specified topic. In addition to receiving a list of current publishers, the subscriber will also receive notifications of new publishers via the publisherUpdate API.
        /// </summary>
        /// <param name="callerId"> ROS caller ID </param>
        /// <param name="topic"> Fully-qualified name of topic. </param>
        /// <param name="topicType"> Datatype for topic. Must be a package-resource name, i.e. the .msg name. </param>
        /// <param name="callerApi"> API URI of subscriber to register. Will be used for new publisher notifications. </param>
        /// <returns> list of XMLRPC API URIs for nodes currently publishing the specified topic. </returns>
        public async Task<List<Uri>> RegisterSubscriberAsync(string callerId, string topic, string topicType, Uri callerApi)
        {
            var result = await Task<object[]>.Factory.FromAsync((callback, o) =>
                                                    _proxy.BeginRegisterSubscriber(callerId, topic, topicType, callerApi.ToString(), callback, o),
                                                    _proxy.EndRegisterSubscriber, null);

            if ((StatusCode)result[0] != StatusCode.Success) throw new InvalidOperationException((string)result[1]);
            return ((object[])result[2]).Select(x => new Uri((string)x)).ToList();
        }

        /// <summary>
        ///   Unregister the caller as a publisher of the topic.
        /// </summary>
        /// <param name="callerId"> ROS caller ID </param>
        /// <param name="topic"> Fully-qualified name of topic. </param>
        /// <param name="callerApi"> API URI of service to unregister. Unregistration will only occur if current registration matches. </param>
        /// <returns> Number of Unsubscribed </returns>
        public async Task<int> UnregisterSubscriberAsync(string callerId, string topic, Uri callerApi)
        {
            var result = await Task<object[]>.Factory.FromAsync(_proxy.BeginUnregisterSubscriber, _proxy.EndUnregisterSubscriber, callerId, topic, callerApi.ToString(), null);
            if ((StatusCode)result[0] != StatusCode.Success) throw new InvalidOperationException((string)result[1]);
            return (int)result[2];
        }

        /// <summary>
        ///   Register the caller as a publisher the topic.
        /// </summary>
        /// <param name="callerId"> ROS caller ID </param>
        /// <param name="topic"> Fully-qualified name of topic to register. </param>
        /// <param name="topicType"> Datatype for topic. Must be a package-resource name, i.e. the .msg name. </param>
        /// <param name="callerApi"> API URI of publisher to register. </param>
        /// <returns> List of current subscribers of topic in the form of XMLRPC URIs. </returns>
        public async Task<List<Uri>> RegisterPublisherAsync(string callerId, string topic, string topicType, Uri callerApi)
        {
            var result = await Task<object[]>.Factory.FromAsync((callback, o) =>
                                                    _proxy.BeginRegisterPublisher(callerId, topic, topicType, callerApi.ToString(), callback, o),
                                                    _proxy.EndRegisterPublisher, null);
            if ((StatusCode)result[0] != StatusCode.Success) throw new InvalidOperationException((string)result[1]);
            return ((object[])result[2]).Select(x => new Uri((string)x)).ToList();
        }

        /// <summary>
        ///   Unregister the caller as a publisher of the topic.
        /// </summary>
        /// <param name="callerId"> ROS caller ID </param>
        /// <param name="topic"> Fully-qualified name of topic to unregister. </param>
        /// <param name="callerApi"> API URI of publisher to unregister. Unregistration will only occur if current registration matches. </param>
        /// <returns> Number of Unregistered </returns>
        public async Task<int> UnregisterPublisherAsync(string callerId, string topic, Uri callerApi)
        {
            var result = await Task<object[]>.Factory.FromAsync(_proxy.BeginUnregisterPublisher, _proxy.EndUnregisterPublisher, callerId, topic, callerApi.ToString(), null);
            if ((StatusCode)result[0] != StatusCode.Success) throw new InvalidOperationException((string)result[1]);
            return (int)result[2];
        }

        /// <summary>
        ///   Get the XML-RPC URI of the node with the associated name/caller_id. This API is for looking information about publishers and subscribers. Use lookupService instead to lookup ROS-RPC URIs.
        /// </summary>
        /// <param name="callerId"> ROS Caller ID </param>
        /// <param name="nodeName"> Name of node to lookup </param>
        /// <returns> URI of the Node </returns>
        public async Task<Uri> LookupNodeAsync(string callerId, string nodeName)
        {
            var result = await Task<object[]>.Factory.FromAsync(_proxy.BeginLookupNode, _proxy.EndLookupNode, callerId, nodeName, null);
            if ((StatusCode)result[0] != StatusCode.Success) throw new InvalidOperationException((string)result[1]);
            return new Uri((string)result[2]);
        }

        /// <summary>
        ///   Get list of topics that can be subscribed to. This does not return topics that have no publishers. See getSystemState() to get more comprehensive list.
        /// </summary>
        /// <param name="callerId"> ROS caller ID </param>
        /// <param name="subgraph"> Restrict topic names to match within the specified subgraph. Subgraph namespace is resolved relative to the caller's namespace. Use emptry string to specify all names. </param>
        /// <returns> TopicInfo list </returns>
        public async Task<List<TopicInfo>> GetPublisherTopicsAsync(string callerId, string subgraph)
        {
            var result = await Task<object[]>.Factory.FromAsync(_proxy.BeginGetPublisherTopics, _proxy.EndGetPublisherTopics, callerId, subgraph, null);
            if ((StatusCode)result[0] != StatusCode.Success) throw new InvalidOperationException((string)result[1]);
            return ((object[])result[2]).Select(x => new TopicInfo()
            {
                TopicName = (string)((object[])x)[0],
                TypeName = (string)((object[])x)[1]
            }).ToList();
        }

        /// <summary>
        ///   Retrieve list representation of system state (i.e. publishers, subscribers, and services).
        /// </summary>
        /// <param name="callerId"> ROS Caller ID </param>
        /// <returns> System state </returns>
        public async Task<SystemState> GetSystemStateAsync(string callerId)
        {
            var result = await Task<object[]>.Factory.FromAsync(_proxy.BeginGetSystemState, _proxy.EndGetSystemState, callerId, null);

            if ((StatusCode)result[0] != StatusCode.Success) throw new InvalidOperationException((string)result[1]);

            var state = (object[])result[2];

            var ret = new SystemState();

            if (state[0] is object[])
            {
                ret.Publishers = ((object[])state[0])
                    .Select(x => new PublisherSystemState()
                    {
                        TopicName = (string)((object[])x)[0],
                        Publishers = ((object[])((object[])x)[1]).Cast<string>().ToList()
                    }).ToList();
            }
            if (state[1] is object[])
            {

                ret.Subscribers = ((object[])state[1])
                    .Select(x => new SubscriberSystemState()
                    {
                        TopicName = (string)((object[])x)[0],
                        Subscribers = ((object[])((object[])x)[1]).Cast<string>().ToList()
                    }).ToList();
            }
            if (state[2] is object[])
            {

                ret.Services = ((object[])state[2])
                    .Select(x => new ServiceSystemState()
                    {
                        ServiceName = (string)((object[])x)[0],
                        Services = ((object[])((object[])x)[1]).Cast<string>().ToList()
                    }).ToList();
            }

            return ret;
        }

        /// <summary>
        ///   Get the URI of the the master.
        /// </summary>
        /// <param name="callerId"> ROS Caller ID </param>
        /// <returns> URI of the master </returns>
        public async Task<Uri> GetUriAsync(string callerId)
        {
            var result = await Task<object[]>.Factory.FromAsync(_proxy.BeginGetUri, _proxy.EndGetUri, callerId, null);
            if ((StatusCode)result[0] != StatusCode.Success) throw new InvalidOperationException((string)result[1]);
            return new Uri((string)result[2]);
        }

        /// <summary>
        ///   Lookup all provider of a particular service.
        /// </summary>
        /// <param name="callerId"> ROS caller ID </param>
        /// <param name="service"> Fully-qualified name of service </param>
        /// <returns> URI of the service </returns>
        public async Task<Uri> LookupServiceAsync(string callerId, string service)
        {
            var result = await Task<object[]>.Factory.FromAsync(_proxy.BeginLookupService, _proxy.EndLookupService, callerId, service, null);
            if ((StatusCode)result[0] != StatusCode.Success) throw new InvalidOperationException((string)result[1]);
            return new Uri((string)result[2]);
        }


        ///////////////////////////////////////////////////////////////
        // External API
        ///////////////////////////////////////////////////////////////

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
    }

    public sealed class TopicInfo
    {
        public string TopicName { get; set; }
        public string TypeName { get; set; }
    }

    public sealed class SystemState
    {
        public List<PublisherSystemState> Publishers { get; set; }
        public List<SubscriberSystemState> Subscribers { get; set; }
        public List<ServiceSystemState> Services { get; set; }
    }

    public sealed class PublisherSystemState
    {
        public string TopicName { get; set; }
        public List<string> Publishers { get; set; }
    }

    public sealed class SubscriberSystemState
    {
        public string TopicName { get; set; }
        public List<string> Subscribers { get; set; }
    }

    public sealed class ServiceSystemState
    {
        public string ServiceName { get; set; }
        public List<string> Services { get; set; }
    }
}