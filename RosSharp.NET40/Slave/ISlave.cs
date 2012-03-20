using CookComputing.XmlRpc;

namespace RosSharp.Slave
{
    /// <summary>
    /// Defines interface for Slave API
    /// </summary>
    /// <remarks>
    /// http://www.ros.org/wiki/ROS/Slave_API
    /// </remarks>
    [XmlRpcUrl("")]
    internal interface ISlave
    {
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
        [XmlRpcMethod("getBusStats")]
        object[] GetBusStats(string callerId);
        
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
        [XmlRpcMethod("getBusInfo")]
        object[] GetBusInfo(string callerId);
        
        /// <summary>
        /// Get the URI of the master node.
        /// </summary>
        /// <param name="callerId">ROS caller ID.</param>
        /// <returns>
        /// int: code
        /// str: status message
        /// str: URI of the master
        /// </returns>
        [XmlRpcMethod("getMasterUri")]
        object[] GetMasterUri(string callerId);
        
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
        [XmlRpcMethod("shutdown")]
        object[] Shutdown(string callerId, string msg);
        
        /// <summary>
        /// Get the PID of this server.
        /// </summary>
        /// <param name="callerId">ROS caller ID.</param>
        /// <returns>
        /// int: code
        /// str: status message
        /// int: server process pid
        /// </returns>
        [XmlRpcMethod("getPid")]
        object[] GetPid(string callerId);
        
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
        [XmlRpcMethod("getSubscriptions")]
        object[] GetSubscriptions(string callerId);
        
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
        [XmlRpcMethod("getPublications")]
        object[] GetPublications(string callerId);
        
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
        [XmlRpcMethod("paramUpdate")]
        object[] ParamUpdate(string callerId, string parameterKey, object parameterValue);
        
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
        [XmlRpcMethod("publisherUpdate")]
        object[] PublisherUpdate(string callerId, string topic, string[] publishers);
        
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
        [XmlRpcMethod("requestTopic")]
        object[] RequestTopic(string callerId, string topic, object[] protocols);
    }
}