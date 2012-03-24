using CookComputing.XmlRpc;

namespace RosSharp.Parameter
{
    /// <summary>
    /// Defines interface for ParameterServer API
    /// </summary>
    /// <remarks>
    /// http://www.ros.org/wiki/ROS/Parameter%20Server%20API
    /// </remarks>
    [XmlRpcUrl("")]
    internal interface IParameterServer
    {
        /// <summary>
        /// Delete parameter
        /// </summary>
        /// <param name="callerId">ROS caller ID</param>
        /// <param name="key">Parameter name.</param>
        /// <returns>
        /// int: code
        /// str: status message
        /// int: ignore
        /// </returns>
        [XmlRpcMethod("deleteParam")]
        object[] DeleteParam(string callerId, string key);

        /// <summary>
        /// Set parameter.
        /// </summary>
        /// <param name="callerId">ROS caller ID</param>
        /// <param name="key">Parameter name.</param>
        /// <param name="value">Parameter value.</param>
        /// <returns>
        /// int: code
        /// str: status message
        /// int: ignore
        /// </returns>
        [XmlRpcMethod("setParam")]
        object[] SetParam(string callerId, string key, object value);

        /// <summary>
        /// Retrieve parameter value from server.
        /// </summary>
        /// <param name="callerId">ROS caller ID</param>
        /// <param name="key">Parameter name. If key is a namespace, getParam() will return a parameter tree.</param>
        /// <returns>
        /// int: code
        /// str: status message
        /// parameterValue
        /// </returns>
        [XmlRpcMethod("getParam")]
        object[] GetParam(string callerId, string key);

        /// <summary>
        /// Search for parameter key on the Parameter Server.
        /// </summary>
        /// <param name="callerId">ROS caller ID</param>
        /// <param name="key">Parameter name to search for.</param>
        /// <returns>
        /// int: code
        /// str: status message
        /// str: foundKey
        /// </returns>
        [XmlRpcMethod("searchParam")]
        object[] SearchParam(string callerId, string key);

        /// <summary>
        /// Retrieve parameter value from server and subscribe to updates to that param.
        /// See paramUpdate() in the Node API.
        /// </summary>
        /// <param name="callerId">ROS caller ID.</param>
        /// <param name="callerApi">Node API URI of subscriber for paramUpdate callbacks.</param>
        /// <param name="key">Parameter name</param>
        /// <returns>
        /// int: code
        /// str: status message
        /// parameterValue
        /// </returns>
        [XmlRpcMethod("subscribeParam")]
        object[] SubscribeParam(string callerId, string callerApi, string key);

        /// <summary>
        /// Retrieve parameter value from server and subscribe to updates to that param. 
        /// See paramUpdate() in the Node API.
        /// </summary>
        /// <param name="callerId">ROS caller ID.</param>
        /// <param name="callerApi">Node API URI of subscriber.</param>
        /// <param name="key">Parameter name.</param>
        /// <returns>
        /// int: code
        /// str: status message
        /// int: number of unsubscribed
        /// </returns>
        [XmlRpcMethod("unsubscribeParam")]
        object[] UnsubscribeParam(string callerId, string callerApi, string key);

        /// <summary>
        /// Check if parameter is stored on server.
        /// </summary>
        /// <param name="callerId">ROS caller ID.</param>
        /// <param name="key">Parameter name.</param>
        /// <returns>
        /// int: code
        /// str: status message
        /// bool: hasParam
        /// </returns>
        [XmlRpcMethod("hasParam")]
        object[] HasParam(string callerId, string key);

        /// <summary>
        /// Get list of all parameter names stored on this server.
        /// </summary>
        /// <param name="callerId">ROS caller ID.</param>
        /// <returns>
        /// int: code
        /// str: status message
        /// str[]: parameter name list
        /// </returns>
        [XmlRpcMethod("getParamNames")]
        object[] GetParamNames(string callerId);
    }
}