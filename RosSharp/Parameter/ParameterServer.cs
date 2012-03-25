using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using CookComputing.XmlRpc;

namespace RosSharp.Parameter
{
    /// <summary>
    /// XML-RPC Server for ParameterServer API
    /// </summary>
    public sealed class ParameterServer : MarshalByRefObject, IParameterServer, IDisposable
    {
        //TODO: サーバ実装を委譲してinternalクラスにしたほうがよいか。

        public Uri ParameterServerUri { get; private set; }
        public ParameterServer(int portNumber)
        {
            var channel = new HttpServerChannel("param", portNumber, new XmlRpcServerFormatterSinkProvider());
            var tmp = new Uri(channel.GetChannelUri());

            ParameterServerUri = new Uri("http://" + ROS.HostName + ":" + tmp.Port + "/param");

            ChannelServices.RegisterChannel(channel, false);
            RemotingServices.Marshal(this, "param");
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

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
        public object[] DeleteParam(string callerId, string key)
        {
            throw new NotImplementedException();
        }

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
        public object[] SetParam(string callerId, string key, object value)
        {
            throw new NotImplementedException();
        }

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
        public object[] GetParam(string callerId, string key)
        {
            throw new NotImplementedException();
        }

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
        public object[] SearchParam(string callerId, string key)
        {
            throw new NotImplementedException();
        }

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
        public object[] SubscribeParam(string callerId, string callerApi, string key)
        {
            throw new NotImplementedException();
        }

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
        public object[] UnsubscribeParam(string callerId, string callerApi, string key)
        {
            throw new NotImplementedException();
        }

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
        public object[] HasParam(string callerId, string key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get list of all parameter names stored on this server.
        /// </summary>
        /// <param name="callerId">ROS caller ID.</param>
        /// <returns>
        /// int: code
        /// str: status message
        /// str[]: parameter name list
        /// </returns>
        public object[] GetParamNames(string callerId)
        {
            throw new NotImplementedException();
        }

    }
}
