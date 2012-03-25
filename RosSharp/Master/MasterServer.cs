using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using Common.Logging;
using CookComputing.XmlRpc;
using RosSharp.Parameter;
using RosSharp.Slave;

namespace RosSharp.Master
{
    /// <summary>
    /// XML-RPC Server for Master API
    /// </summary>
    public sealed class MasterServer : MarshalByRefObject, IMaster,IParameterServer, IDisposable
    {
        //TODO: サーバ実装を委譲してinternalクラスにしたほうがよいか。

        private readonly RegistrationContainer _registrationContainer = new RegistrationContainer();

        public Uri MasterUri { get; private set; }

        private readonly HttpServerChannel _channel;

        private ILog _logger = LogManager.GetCurrentClassLogger();

        private ParameterServer _parameterServer;

        public MasterServer(int portNumber)
        {
            _channel = new HttpServerChannel("master", portNumber, new XmlRpcServerFormatterSinkProvider());

            var tmp = new Uri(_channel.GetChannelUri());

            MasterUri = new Uri("http://" + ROS.HostName + ":" + tmp.Port);

            ChannelServices.RegisterChannel(_channel, false);
            RemotingServices.Marshal(this, "/");

            _parameterServer = new ParameterServer(MasterUri);
        }

        public void Dispose()
        {
            ChannelServices.UnregisterChannel(_channel);
            //RemotingServices.Unmarshal(this);
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        /// <summary>
        /// Register the caller as a provider of the specified service.
        /// </summary>
        /// <param name="callerId">ROS Caller ID</param>
        /// <param name="service">Fully-qualified name of service</param>
        /// <param name="serviceApi">ROSRPC Service URI</param>
        /// <param name="callerApi">XML-RPC URI of caller node</param>
        /// <returns>
        /// int: code
        /// str: status message
        /// int: ignore
        /// </returns>
        public object[] RegisterService(string callerId, string service, string serviceApi, string callerApi)
        {
            _logger.Debug(m => m("RegisterService(callerId={0},service={1},serviceApi={2},callerApi={3})"
                                 , callerId, service, serviceApi, callerApi));

            if (string.IsNullOrEmpty(service))
            {
                return new object[3]
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
                    _registrationContainer.RegisterService(service, new Uri(serviceApi), new Uri(callerApi));
                }
                return new object[3]
                {
                    StatusCode.Success,
                    "Regisered [" + callerId + "] as provider of [" + service + "]",
                    1
                };
            }
            catch (UriFormatException ex)
            {
                return new object[3]
                {
                    StatusCode.Error,
                    "ERROR: parameter ["+ serviceApi + "] is not an XMLRPC URI",
                    0
                };
            }
        }

        /// <summary>
        /// Unregister the caller as a provider of the specified service.
        /// </summary>
        /// <param name="callerId">ROS Caller ID</param>
        /// <param name="service">Fully-qualified name of service</param>
        /// <param name="serviceApi">API URI of service to unregister.
        /// Unregistration will only occur if current registration matches.</param>
        /// <returns>
        /// int: code
        /// str: status message
        /// int: Number of Unregistrations
        /// </returns>
        public object[] UnregisterService(string callerId, string service, string serviceApi)
        {
            _logger.Debug(m => m("UnregisterService(callerId={0},service={1},serviceApi={2})"
                                 , callerId, service, serviceApi));

            if (string.IsNullOrEmpty(service))
            {
                return new object[3]
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
                    return new object[3]
                    {
                        StatusCode.Success,
                        "Unregistered [" + callerId + "] as provider of [" + service + "]",
                        1
                    };
                }
                else
                {
                    return new object[3]
                    {
                        StatusCode.Success,
                        "[" + service + "] is not a registered node",
                        0
                    };
                }
            }
            catch (UriFormatException ex)
            {
                return new object[3]
                {
                    StatusCode.Error,
                    "ERROR: parameter ["+ serviceApi + "] is not an XMLRPC URI",
                    0
                };
            }
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
        /// <returns>
        /// int: code
        /// str: status message
        /// str[]: list of XMLRPC API URIs for nodes currently publishing the specified topic.
        /// </returns>
        public object[] RegisterSubscriber(string callerId, string topic, string topicType, string callerApi)
        {
            _logger.Debug(m => m("RegisterSubscriber(callerId={0},topic={1},topicType={2},callerApi={3})"
                                 , callerId, topic, topicType, callerApi));

            if (string.IsNullOrEmpty(topic))
            {
                return new object[3]
                {
                    StatusCode.Error,
                    "ERROR: parameter [" + topic + "] must be a non-empty string",
                    0
                };
            }
            if (string.IsNullOrEmpty(topicType))
            {
                return new object[3]
                {
                    StatusCode.Error,
                    "ERROR: parameter [" + topicType + "] must be a non-empty string",
                    0
                };
            }

            try
            {
                List<Uri> uris;
                lock (_registrationContainer)
                {
                    uris = _registrationContainer.RegsiterSubscriber(topic, topicType, new Uri(callerApi));
                }
                return new object[3]
                {
                    StatusCode.Success,
                    "Subscribed to [" + topic + "]",
                    uris.Select(x => x.ToString()).ToArray()
                };
            }
            catch (UriFormatException ex)
            {
                return new object[3]
                {
                    StatusCode.Error,
                    "ERROR: parameter ["+ callerApi + "] is not an XMLRPC URI",
                    0
                };
            }

        }

        /// <summary>
        /// Unregister the caller as a publisher of the topic.
        /// </summary>
        /// <param name="callerId">ROS caller ID</param>
        /// <param name="topic">Fully-qualified name of topic.</param>
        /// <param name="callerApi">API URI of service to unregister.
        /// Unregistration will only occur if current registration matches.</param>
        /// <returns>
        /// int: code
        /// str: status message
        /// int: Number of Unsubscribed
        /// </returns>
        public object[] UnregisterSubscriber(string callerId, string topic, string callerApi)
        {
            _logger.Debug(m => m("RegisterSubscriber(callerId={0},topic={1},callerApi={2})"
                                 , callerId, topic, callerApi));

            if (string.IsNullOrEmpty(topic))
            {
                return new object[3]
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
                    return new object[3]
                    {
                        StatusCode.Success,
                        "Unregistered [" + callerId + "] as topic of [" + topic + "]",
                        1
                    };
                }
                else
                {
                    return new object[3]
                    {
                        StatusCode.Success,
                        "[" + topic + "] is not a registered node",
                        0
                    };
                }
            }
            catch (UriFormatException ex)
            {
                return new object[3]
                {
                    StatusCode.Error,
                    "ERROR: parameter ["+ callerApi + "] is not an XMLRPC URI",
                    0
                };
            }
        }

        /// <summary>
        /// Register the caller as a publisher the topic.
        /// </summary>
        /// <param name="callerId">ROS caller ID</param>
        /// <param name="topic">Fully-qualified name of topic to register.</param>
        /// <param name="topicType">Datatype for topic. Must be a package-resource name, i.e. the .msg name.</param>
        /// <param name="callerApi">API URI of publisher to register.</param>
        /// <returns>
        /// int: code
        /// str: status message
        /// str[]: List of current subscribers of topic in the form of XMLRPC URIs.
        /// </returns>
        public object[] RegisterPublisher(string callerId, string topic, string topicType, string callerApi)
        {
            _logger.Debug(m => m("RegisterPublisher(callerId={0},topic={1},topicType={2},callerApi={3})"
                                 , callerId, topic, topicType, callerApi));

            if (string.IsNullOrEmpty(topic))
            {
                return new object[3]
                {
                    StatusCode.Error,
                    "ERROR: parameter [" + topic + "] must be a non-empty string",
                    0
                };
            }
            if (string.IsNullOrEmpty(topicType))
            {
                return new object[3]
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
                    info = _registrationContainer.RegisterPublisher(topic, topicType, new Uri(callerApi));
                    UpdatePublisher(info);
                }
                return new object[3]
                {
                    1,
                    "Registered [" + callerId + "] as publisher of [" + topic + "]",
                    info.SubscriberUris.Select(x => x.ToString()).ToArray()
                };
            }
            catch (UriFormatException ex)
            {
                return new object[3]
                {
                    StatusCode.Error,
                    "ERROR: parameter ["+ callerApi + "] is not an XMLRPC URI",
                    0
                };
            }
        }

        private void UpdatePublisher(TopicRegistrationInfo info)
        {
            var slaves = info.SubscriberUris.Select(uri => new SlaveClient(uri));

            var publishers = info.PublisherUris.Select(x => x.ToString()).ToArray();

            //TODO: Firstはだめ？Serverの中なのでどうすべきか。Java版では接続せずに帰ってきてる感じ。
            slaves.ToList().ForEach(s => s.PublisherUpdateAsync("", info.TopicName, publishers).First());
        }

        /// <summary>
        /// Unregister the caller as a publisher of the topic.
        /// </summary>
        /// <param name="callerId">ROS caller ID</param>
        /// <param name="topic">Fully-qualified name of topic to unregister.</param>
        /// <param name="callerApi">API URI of publisher to unregister.
        /// Unregistration will only occur if current registration matches.</param>
        /// <returns>
        /// int: code
        /// str: status message
        /// int: Number of Unregistered
        /// </returns>
        public object[] UnregisterPublisher(string callerId, string topic, string callerApi)
        {
            _logger.Debug(m => m("UnregisterPublisher(callerId={0},topic={1},callerApi={2})"
                                 , callerId, topic, callerApi));

            if (string.IsNullOrEmpty(topic))
            {
                return new object[3]
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
                    return new object[3]
                    {
                        StatusCode.Success,
                        "Unregistered [" + callerId + "] as topic of [" + topic + "]",
                        1
                    };
                }
                else
                {
                    return new object[3]
                    {
                        StatusCode.Success,
                        "[" + topic + "] is not a registered node",
                        0
                    };
                }
            }
            catch (UriFormatException ex)
            {
                return new object[3]
                {
                    StatusCode.Error,
                    "ERROR: parameter ["+ callerApi + "] is not an XMLRPC URI",
                    0
                };
            }
        }

        /// <summary>
        /// Get the XML-RPC URI of the node with the associated name/caller_id.
        /// This API is for looking information about publishers and subscribers.
        /// Use lookupService instead to lookup ROS-RPC URIs.
        /// </summary>
        /// <param name="callerId">ROS Caller ID</param>
        /// <param name="nodeName">Name of node to lookup</param>
        /// <returns>
        /// int: code
        /// str: status message
        /// str: URI of the Node
        /// </returns>
        public object[] LookupNode(string callerId, string nodeName)
        {
            _logger.Debug(m => m("UnregisterPublisher(callerId={0},nodeName={1})", callerId, nodeName));

            if (string.IsNullOrEmpty(nodeName))
            {
                return new object[3]
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
                return new object[3]
                {
                    StatusCode.Success,
                    "node api: [" + uri + "]",
                    uri.ToString()
                };
            }
            else
            {
                return new object[3]
                {
                    StatusCode.Error,
                    "unknown node [" + nodeName + "]",
                    ""
                };
            }

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
        /// <returns>
        /// int: code
        /// str: status message
        /// str[][]: [topic1, type1]...[topicN, typeN]
        /// </returns>
        public object[] GetPublisherTopics(string callerId, string subgraph)
        {
            _logger.Debug(m => m("GetPublisherTopics(callerId={0},subgraph={1})", callerId, subgraph));

            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieve list representation of system state (i.e. publishers, subscribers, and services).
        /// </summary>
        /// <param name="callerId">ROS Caller ID</param>
        /// <returns>
        /// int: code
        /// str: status message
        /// System state is in list representation
        ///   [publishers, subscribers, services]:
        ///     publishers: [ [topic1, [topic1Publisher1...topic1PublisherN]] ... ]
        ///     subscribers: [ [topic1, [topic1Subscriber1...topic1SubscriberN]] ... ]
        ///     services: [ [service1, [service1Provider1...service1ProviderN]] ... ]
        /// </returns>
        public object[] GetSystemState(string callerId)
        {
            _logger.Debug(m => m("GetSystemState(callerId={0})", callerId));
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the URI of the the master.
        /// </summary>
        /// <param name="callerId">ROS Caller ID</param>
        /// <returns>
        /// int: code
        /// str: status message
        /// str: URI of the master
        /// </returns>
        public object[] GetUri(string callerId)
        {
            _logger.Debug(m => m("GetUri(callerId={0})", callerId));

            return new object[3]
            {
                StatusCode.Success,
                "",
                MasterUri.ToString()
            };
        }

        /// <summary>
        /// Lookup all provider of a particular service.
        /// </summary>
        /// <param name="callerId">ROS caller ID</param>
        /// <param name="service">Fully-qualified name of service</param>
        /// <returns>
        /// int: code
        /// str: status message
        /// str: URI of the service
        /// </returns>
        public object[] LookupService(string callerId, string service)
        {
            _logger.Debug(m => m("LookupService(callerId={0},service={1})", callerId, service));

            Uri uri;
            lock (_registrationContainer)
            {
                uri = _registrationContainer.LookUpService(service);
            }

            if (uri != null)
            {
                return new object[3]
                {
                    StatusCode.Success,
                    "rosrpc URI: [" + uri + "]",
                    uri.ToString()
                };
            }
            else
            {
                return new object[3]
                {
                    StatusCode.Error,
                    "no provider",
                    ""
                };
            }
        }

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
    }

}
