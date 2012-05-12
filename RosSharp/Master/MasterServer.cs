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
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using CookComputing.XmlRpc;
using RosSharp.Parameter;
using RosSharp.Slave;

namespace RosSharp.Master
{
    /// <summary>
    ///   XML-RPC Server for Master API
    /// </summary>
    public sealed class MasterServer : MarshalByRefObject, IMaster, IParameterServer, IDisposable
    {
        private readonly HttpServerChannel _channel;
        private readonly RegistrationContainer _registrationContainer = new RegistrationContainer();

        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        private readonly ParameterServer _parameterServer;

        public MasterServer(int portNumber)
        {
            _channel = new HttpServerChannel("master", portNumber, new XmlRpcServerFormatterSinkProvider());
            
            var tmp = new Uri(_channel.GetChannelUri());

            MasterUri = new Uri("http://" + Ros.HostName + ":" + tmp.Port);

            ChannelServices.RegisterChannel(_channel, false);
            RemotingServices.Marshal(this, "/");

            _logger.Info(m => m("MasterServer launched {0}", MasterUri.ToString()));

            _parameterServer = new ParameterServer(MasterUri);

        }

        public Uri MasterUri { get; private set; }

        #region IDisposable Members

        public void Dispose()
        {
            ChannelServices.UnregisterChannel(_channel);
            RemotingServices.Disconnect(this);
        }

        #endregion

        #region IMaster Members

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
        public object[] RegisterService(string callerId, string service, string serviceApi, string callerApi)
        {
            _logger.Debug(m => m("RegisterServiceAsync(callerId={0},service={1},serviceApi={2},callerApi={3})"
                                 , callerId, service, serviceApi, callerApi));

            if (string.IsNullOrEmpty(service))
            {
                _logger.Error(m => m("ERROR: parameter [{0}] must be a non-empty string", service));
                return new object[]
                {
                    StatusCode.Error,
                    "ERROR: parameter [" + service + "] must be a non-empty string",
                    0
                };
            }
            try
            {
                lock (_registrationContainer)
                {
                    _registrationContainer.RegisterService(callerId, service, new Uri(serviceApi), new Uri(callerApi));
                }
                return new object[]
                {
                    StatusCode.Success,
                    "Regisered [" + callerId + "] as provider of [" + service + "]",
                    1
                };
            }
            catch (UriFormatException ex)
            {
                _logger.Error(m => m("ERROR: parameter [{0}] is not an XMLRPC URI", serviceApi), ex);
                return new object[]
                {
                    StatusCode.Error,
                    "ERROR: parameter [" + serviceApi + "] is not an XMLRPC URI",
                    0
                };
            }
        }

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
        public object[] UnregisterService(string callerId, string service, string serviceApi)
        {
            _logger.Debug(m => m("UnregisterService(callerId={0},service={1},serviceApi={2})"
                                 , callerId, service, serviceApi));

            if (string.IsNullOrEmpty(service))
            {
                _logger.Error(m => m("ERROR: parameter [{0}] must be a non-empty string", service));
                return new object[]
                {
                    StatusCode.Error,
                    "ERROR: parameter [" + service + "] must be a non-empty string",
                    0
                };
            }
            try
            {
                bool success;
                lock (_registrationContainer)
                {
                    success = _registrationContainer.UnregisterService(service, new Uri(serviceApi));
                }
                if (success)
                {
                    return new object[]
                    {
                        StatusCode.Success,
                        "Unregistered [" + callerId + "] as provider of [" + service + "]",
                        1
                    };
                }
                else
                {
                    _logger.Error(m => m("[{0}] is not a registered node", service));
                    return new object[]
                    {
                        StatusCode.Success,
                        "[" + service + "] is not a registered node",
                        0
                    };
                }
            }
            catch (UriFormatException ex)
            {
                _logger.Error(m => m("ERROR: parameter [{0}] is not an XMLRPC URI", serviceApi), ex);
                return new object[]
                {
                    StatusCode.Error,
                    "ERROR: parameter [" + serviceApi + "] is not an XMLRPC URI",
                    0
                };
            }
        }

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
        public object[] RegisterSubscriber(string callerId, string topic, string topicType, string callerApi)
        {
            _logger.Debug(m => m("RegisterSubscriber(callerId={0},topic={1},topicType={2},callerApi={3})"
                                 , callerId, topic, topicType, callerApi));

            if (string.IsNullOrEmpty(topic))
            {
                _logger.Error(m => m("ERROR: parameter [{0}] must be a non-empty string", topic));
                return new object[]
                {
                    StatusCode.Error,
                    "ERROR: parameter [" + topic + "] must be a non-empty string",
                    0
                };
            }
            if (string.IsNullOrEmpty(topicType))
            {
                _logger.Error(m => m("ERROR: parameter [{0}] must be a non-empty string", topicType));
                return new object[]
                {
                    StatusCode.Error,
                    "ERROR: parameter [" + topicType + "] must be a non-empty string",
                    0
                };
            }

            try
            {
                List<RegistrationInfo> infos;
                lock (_registrationContainer)
                {
                    infos = _registrationContainer.RegisterSubscriber(callerId, topic, topicType, new Uri(callerApi));
                }
                return new object[]
                {
                    StatusCode.Success,
                    "Subscribed to [" + topic + "]",
                    infos.Select(x => x.Uri.ToString()).ToArray()
                };
            }
            catch (UriFormatException ex)
            {
                _logger.Error(m => m("ERROR: parameter [{0}] is not an XMLRPC URI", callerApi), ex);
                return new object[]
                {
                    StatusCode.Error,
                    "ERROR: parameter [" + callerApi + "] is not an XMLRPC URI",
                    0
                };
            }
        }

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
        public object[] UnregisterSubscriber(string callerId, string topic, string callerApi)
        {
            _logger.Debug(m => m("RegisterSubscriber(callerId={0},topic={1},callerApi={2})"
                                 , callerId, topic, callerApi));

            if (string.IsNullOrEmpty(topic))
            {
                _logger.Error(m => m("ERROR: parameter [{0}] must be a non-empty string", topic));
                return new object[]
                {
                    StatusCode.Error,
                    "ERROR: parameter [" + topic + "] must be a non-empty string",
                    0
                };
            }

            try
            {
                bool success;
                lock (_registrationContainer)
                {
                    success = _registrationContainer.UnregisterSubscriber(topic, new Uri(callerApi));
                }
                if (success)
                {
                    return new object[]
                    {
                        StatusCode.Success,
                        "Unregistered [" + callerId + "] as topic of [" + topic + "]",
                        1
                    };
                }
                else
                {
                    _logger.Error(m => m("[{0}] is not a registered node", topic));
                    return new object[]
                    {
                        StatusCode.Success,
                        "[" + topic + "] is not a registered node",
                        0
                    };
                }
            }
            catch (UriFormatException ex)
            {
                _logger.Error(m => m("ERROR: parameter [{0}] is not an XMLRPC URI", callerApi), ex);
                return new object[]
                {
                    StatusCode.Error,
                    "ERROR: parameter [" + callerApi + "] is not an XMLRPC URI",
                    0
                };
            }
        }

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
        public object[] RegisterPublisher(string callerId, string topic, string topicType, string callerApi)
        {
            _logger.Debug(m => m("RegisterPublisher(callerId={0},topic={1},topicType={2},callerApi={3})"
                                 , callerId, topic, topicType, callerApi));

            if (string.IsNullOrEmpty(topic))
            {
                _logger.Error(m => m("ERROR: parameter [{0}] must be a non-empty string", topic));
                return new object[]
                {
                    StatusCode.Error,
                    "ERROR: parameter [" + topic + "] must be a non-empty string",
                    0
                };
            }
            if (string.IsNullOrEmpty(topicType))
            {
                _logger.Error(m => m("ERROR: parameter [{0}] must be a non-empty string", topicType));
                return new object[]
                {
                    StatusCode.Error,
                    "ERROR: parameter [" + topicType + "] must be a non-empty string",
                    0
                };
            }

            try
            {
                TopicRegistrationInfo info;
                lock (_registrationContainer)
                {
                    info = _registrationContainer.RegisterPublisher(callerId, topic, topicType, new Uri(callerApi));
                    UpdatePublisher(callerId, info);
                }

                _logger.Debug("Registered Publisher");
                return new object[]
                {
                    StatusCode.Success,
                    "Registered [" + callerId + "] as publisher of [" + topic + "]",
                    info.Subscribers.Select(x => x.Uri.ToString()).ToArray()
                };
            }
            catch (UriFormatException ex)
            {
                _logger.Error(m => m("ERROR: parameter [{0}] is not an XMLRPC URI", callerApi), ex);
                return new object[]
                {
                    StatusCode.Error,
                    "ERROR: parameter [" + callerApi + "] is not an XMLRPC URI",
                    0
                };
            }
        }

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
        public object[] UnregisterPublisher(string callerId, string topic, string callerApi)
        {
            _logger.Debug(m => m("UnregisterPublisher(callerId={0},topic={1},callerApi={2})"
                                 , callerId, topic, callerApi));

            if (string.IsNullOrEmpty(topic))
            {
                _logger.Error(m => m("ERROR: parameter [{0}] must be a non-empty string", topic));
                return new object[]
                {
                    StatusCode.Error,
                    "ERROR: parameter [" + topic + "] must be a non-empty string",
                    0
                };
            }

            try
            {
                bool success;
                lock (_registrationContainer)
                {
                    success = _registrationContainer.UnregisterPublisher(topic, new Uri(callerApi));
                }
                if (success)
                {
                    return new object[]
                    {
                        StatusCode.Success,
                        "Unregistered [" + callerId + "] as topic of [" + topic + "]",
                        1
                    };
                }
                else
                {
                    _logger.Error(m => m("[{0}] is not a registered node", topic));
                    return new object[]
                    {
                        StatusCode.Success,
                        "[" + topic + "] is not a registered node",
                        0
                    };
                }
            }
            catch (UriFormatException ex)
            {
                _logger.Error(m => m("ERROR: parameter [{0}] is not an XMLRPC URI", callerApi), ex);
                return new object[]
                {
                    StatusCode.Error,
                    "ERROR: parameter [" + callerApi + "] is not an XMLRPC URI",
                    0
                };
            }
        }

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
        public object[] LookupNode(string callerId, string nodeName)
        {
            _logger.Debug(m => m("UnregisterPublisher(callerId={0},nodeName={1})", callerId, nodeName));

            if (string.IsNullOrEmpty(nodeName))
            {
                _logger.Error(m => m("ERROR: parameter [{0}] must be a non-empty string", nodeName));
                return new object[]
                {
                    StatusCode.Error,
                    "ERROR: parameter [" + nodeName + "] must be a non-empty string",
                    0
                };
            }
            Uri uri;
            lock (_registrationContainer)
            {
                uri = _registrationContainer.LookUpNode(nodeName);
            }

            if (uri != null)
            {
                return new object[]
                {
                    StatusCode.Success,
                    "node api: [" + uri + "]",
                    uri.ToString()
                };
            }
            else
            {
                _logger.Error(m => m("unknown node [{0}]", nodeName));
                return new object[]
                {
                    StatusCode.Error,
                    "unknown node [" + nodeName + "]",
                    ""
                };
            }
        }

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
        public object[] GetPublisherTopics(string callerId, string subgraph)
        {
            _logger.Debug(m => m("GetPublisherTopics(callerId={0},subgraph={1})", callerId, subgraph));

            return new object[]
            {
                StatusCode.Error,
                "method \"getPublisherTopics\" is not supported",
                ""
            };
        }

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
        public object[] GetSystemState(string callerId)
        {
            _logger.Debug(m => m("GetSystemState(callerId={0})", callerId));

            var pubs = _registrationContainer.GetPublishers();
            var subs = _registrationContainer.GetSubscribers();
            var srvs = _registrationContainer.GetServices();

            var stats = new object[3];

            stats[0] = pubs.Select(pub => new object[]
            {
                pub.TopicName,
                pub.Publishers.Select(x => x.NodeId).ToArray()
            }).ToArray();

            stats[1] = subs.Select(sub => new object[]
            {
                sub.TopicName,
                sub.Subscribers.Select(x => x.NodeId).ToArray()
            }).ToArray();

            stats[2] = srvs.Select(srv => new object[]
            {
                srv.ServiceName,
                new string[] {srv.Service.NodeId}
            }).ToArray();

            return new object[]
            {
                StatusCode.Success,
                "current system state",
                stats
            };
        }

        /// <summary>
        ///   Get the URI of the the master.
        /// </summary>
        /// <param name="callerId"> ROS Caller ID </param>
        /// <returns>
        /// [0] = int: code <br/>
        /// [1] = str: status message <br/>
        /// [2] = str: URI of the master
        /// </returns>
        public object[] GetUri(string callerId)
        {
            _logger.Debug(m => m("GetUri(callerId={0})", callerId));

            return new object[]
            {
                StatusCode.Success,
                "",
                MasterUri.ToString()
            };
        }

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
        public object[] LookupService(string callerId, string service)
        {
            _logger.Debug(m => m("LookupService(callerId={0},service={1})", callerId, service));

            ServiceRegistrationInfo info;
            lock (_registrationContainer)
            {
                info = _registrationContainer.LookUpService(service);
            }

            if (info == null)
            {
                _logger.Error("no provider");
                return new object[]
                {
                    StatusCode.Error,
                    "no provider",
                    ""
                };
            }

            return new object[]
            {
                StatusCode.Success,
                "rosrpc URI: [" + info + "]",
                info.Service.Uri.ToString()
            };
        }

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
            //_logger.Debug(m => m("GetPid(callerId={0})", callerId));
            return new object[]
            {
                StatusCode.Success,
                "get pid", //空文字にするとroscppからの呼び出しが失敗する。
                Process.GetCurrentProcess().Id
            };
        }

        #endregion

        #region Implementation of IParameterServer

        public object[] DeleteParam(string callerId, string key)
        {
            return _parameterServer.DeleteParam(callerId, key);
        }

        public object[] SetParam(string callerId, string key, object value)
        {
            return _parameterServer.SetParam(callerId, key, value);
        }

        public object[] GetParam(string callerId, string key)
        {
            return _parameterServer.GetParam(callerId, key);
        }

        public object[] SearchParam(string callerId, string key)
        {
            return _parameterServer.SearchParam(callerId, key);
        }

        public object[] SubscribeParam(string callerId, string callerApi, string key)
        {
            return _parameterServer.SubscribeParam(callerId, callerApi, key);
        }

        public object[] UnsubscribeParam(string callerId, string callerApi, string key)
        {
            return _parameterServer.UnsubscribeParam(callerId, callerApi, key);
        }

        public object[] HasParam(string callerId, string key)
        {
            return _parameterServer.HasParam(callerId, key);
        }

        public object[] GetParamNames(string callerId)
        {
            return _parameterServer.GetParamNames(callerId);
        }

        #endregion

        public override object InitializeLifetimeService()
        {
            return null;
        }

        private void UpdatePublisher(string callerId, TopicRegistrationInfo info)
        {
            var slaves = info.Subscribers.Select(x => new SlaveClient(x.Uri));

            var publishers = info.Publishers.Select(x => x.Uri.ToString()).ToArray();

            _logger.Debug(m => m("UpdatePublisher: slaves={0}, publishers={1}", slaves.Count(), publishers.Length));

            foreach (var slave in slaves)
            {
                slave.PublisherUpdateAsync(callerId, info.TopicName, publishers)
                    .ContinueWith(task =>
                    {
                        _logger.Error("UpdatePublisher: Failure", task.Exception.InnerException);
                    }, TaskContinuationOptions.OnlyOnFaulted);
            }
        }
    }
}