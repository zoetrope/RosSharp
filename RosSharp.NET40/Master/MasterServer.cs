using System;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using CookComputing.XmlRpc;

namespace RosSharp.Master
{
    /// <summary>
    /// XML-RPC Server for Master API
    /// </summary>
    public sealed class MasterServer : MarshalByRefObject, IMaster, IDisposable
    {
        //TODO: サーバ実装を委譲してinternalクラスにしたほうがよいか。

        private readonly RegistrationTopicContainer _registrationTopicContainer;
        private readonly RegistrationServiceContainer _registrationServiceContainer;

        public Uri MasterUri { get; private set; }

        public MasterServer(int portNumber)
        {
            _registrationTopicContainer = new RegistrationTopicContainer();
            _registrationServiceContainer = new RegistrationServiceContainer();


            var channel = new HttpServerChannel("master", portNumber, new XmlRpcServerFormatterSinkProvider());
            
            var tmp = new Uri(channel.GetChannelUri());

            MasterUri = new Uri("http://" + ROS.LocalHostName + ":" + tmp.Port);

            ChannelServices.RegisterChannel(channel, false);
            RemotingServices.Marshal(this, "/");
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
            _registrationServiceContainer.RegisterService(service, new Uri(serviceApi), new Uri(callerApi));

            //TODO: エラー処理
            return new object[3]
            {
                1,
                "Regisered [" + callerId + "] as provider of [" + service + "]",
                1
            };
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
            _registrationServiceContainer.UnregisterService(service, new Uri(serviceApi));

            throw new NotImplementedException();
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
            var uris = _registrationTopicContainer.RegsiterSubscriber(topic, topicType, new Uri(callerApi));

            //TODO: エラー処理
            return new object[3]
            {
                1,
                "Subscribed to [" + topic + "]",
                uris.Select(x => x.ToString()).ToArray()
            };
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
            _registrationTopicContainer.UnregisterSubscriber(topic, new Uri(callerApi));
            throw new NotImplementedException();
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
            var uris = _registrationTopicContainer.RegisterPublisher(topic, topicType, new Uri(callerApi));

            //TODO: エラー処理
            return new object[3]
            {
                1,
                "Registered [" + callerId + "] as publisher of [" + topic + "]",
                uris.Select(x => x.ToString()).ToArray()
            };
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
            _registrationTopicContainer.UnregisterPublisher(topic, new Uri(callerApi));
            throw new NotImplementedException();
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

            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            var uri = _registrationServiceContainer.LookUp(service);

            if (uri != null)
            {
                return new object[3]
                {
                    1,
                    "rosrpc URI: [" + uri + "]",
                    uri.ToString()
                };
            }
            else
            {
                return new object[3]
                {
                    -1,
                    "no provider",
                    ""
                };
            }
        }
    }

}
