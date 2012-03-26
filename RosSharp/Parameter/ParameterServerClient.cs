using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace RosSharp.Parameter
{
    /// <summary>
    /// XML-RPC Client for ParameterServer API
    /// </summary>
    public sealed class ParameterServerClient
    {
        private ParameterServerProxy _proxy;
        public ParameterServerClient(Uri uri)
        {
            _proxy = new ParameterServerProxy();
            _proxy.Url = uri.ToString();
            _proxy.Timeout = ROS.XmlRpcTimeout;
        }

        /// <summary>
        /// Delete parameter
        /// </summary>
        /// <param name="callerId">ROS caller ID</param>
        /// <param name="key">Parameter name.</param>
        /// <returns>ignore</returns>
        public Task DeleteParamAsync(string callerId, string key)
        {
            return Task<object[]>.Factory.FromAsync(_proxy.BeginDeleteParam, _proxy.EndDeleteParam, callerId,key, null)
                .ContinueWith(task =>
                {
                    if ((StatusCode)task.Result[0] != StatusCode.Success) throw new InvalidOperationException((string)task.Result[1]);
                });
        }

        /// <summary>
        /// Set parameter.
        /// </summary>
        /// <param name="callerId">ROS caller ID</param>
        /// <param name="key">Parameter name.</param>
        /// <param name="value">Parameter value.</param>
        /// <returns>ignore</returns>
        public Task SetParamAsync(string callerId, string key, object value)
        {
            return Task<object[]>.Factory.FromAsync(_proxy.BeginSetParam, _proxy.EndSetParam, callerId, key,value, null)
                .ContinueWith(task =>
                {
                    if ((StatusCode)task.Result[0] != StatusCode.Success) throw new InvalidOperationException((string)task.Result[1]);
                });
        }

        /// <summary>
        /// Retrieve parameter value from server.
        /// </summary>
        /// <param name="callerId">ROS caller ID</param>
        /// <param name="key">Parameter name. If key is a namespace, getParam() will return a parameter tree.</param>
        /// <returns>parameterValue</returns>
        public Task<object> GetParamAsync(string callerId, string key)
        {
            return Task<object[]>.Factory.FromAsync(_proxy.BeginGetParam, _proxy.EndGetParam, callerId, key, null)
                .ContinueWith(task =>
                {
                    if ((StatusCode)task.Result[0] != StatusCode.Success) throw new InvalidOperationException((string)task.Result[1]);
                    return task.Result[2];
                 });
        }

        /// <summary>
        /// Search for parameter key on the Parameter Server.
        /// </summary>
        /// <param name="callerId">ROS caller ID</param>
        /// <param name="key">Parameter name to search for.</param>
        /// <returns>foundKey</returns>
        public Task<string> SearchParamAsync(string callerId, string key)
        {
            return Task<object[]>.Factory.FromAsync(_proxy.BeginSearchParam, _proxy.EndSearchParam, callerId, key, null)
                .ContinueWith(task =>
                {
                    if ((StatusCode)task.Result[0] != StatusCode.Success) throw new InvalidOperationException((string)task.Result[1]);
                    return (string) task.Result[2];
                });
        }

        /// <summary>
        /// Retrieve parameter value from server and subscribe to updates to that param.
        /// See paramUpdate() in the Node API.
        /// </summary>
        /// <param name="callerId">ROS caller ID.</param>
        /// <param name="callerApi">Node API URI of subscriber for paramUpdate callbacks.</param>
        /// <param name="key">Parameter name</param>
        /// <returns>parameterValue</returns>
        public Task<object> SubscribeParamAsync(string callerId, Uri callerApi, string key)
        {
            return Task<object[]>.Factory.FromAsync(_proxy.BeginSubscribeParam, _proxy.EndSubscribeParam, callerId, callerApi.ToString(),key, null)
                .ContinueWith(task =>
                {
                    if ((StatusCode)task.Result[0] != StatusCode.Success) throw new InvalidOperationException((string)task.Result[1]);
                    return task.Result[2];
                });
        }

        /// <summary>
        /// Retrieve parameter value from server and subscribe to updates to that param. 
        /// See paramUpdate() in the Node API.
        /// </summary>
        /// <param name="callerId">ROS caller ID.</param>
        /// <param name="callerApi">Node API URI of subscriber.</param>
        /// <param name="key">Parameter name.</param>
        /// <returns>number of unsubscribed</returns>
        public Task<int> UnsubscribeParamAsync(string callerId, Uri callerApi, string key)
        {
            return Task<object[]>.Factory.FromAsync(_proxy.BeginUnsubscribeParam, _proxy.EndUnsubscribeParam, callerId, callerApi.ToString(), key, null)
                .ContinueWith(task =>
                {
                    if ((StatusCode)task.Result[0] != StatusCode.Success) throw new InvalidOperationException((string)task.Result[1]);
                    return (int)task.Result[2];
                });
        }

        /// <summary>
        /// Check if parameter is stored on server.
        /// </summary>
        /// <param name="callerId">ROS caller ID.</param>
        /// <param name="key">Parameter name.</param>
        /// <returns>hasParam</returns>
        public Task<bool> HasParamAsync(string callerId, string key)
        {
            return Task<object[]>.Factory.FromAsync(_proxy.BeginHasParam, _proxy.EndHasParam, callerId, key, null)
                .ContinueWith(task =>
                {
                    if ((StatusCode)task.Result[0] != StatusCode.Success) throw new InvalidOperationException((string)task.Result[1]);
                    return (bool)task.Result[2];
                });
        }

        /// <summary>
        /// Get list of all parameter names stored on this server.
        /// </summary>
        /// <param name="callerId">ROS caller ID.</param>
        /// <returns>parameter name list</returns>
        public Task<List<string>> GetParamNamesAsync(string callerId)
        {
            return Task<object[]>.Factory.FromAsync(_proxy.BeginGetParamNames, _proxy.EndGetParamNames, callerId, null)
                .ContinueWith(task =>
                {
                    if ((StatusCode)task.Result[0] != StatusCode.Success) throw new InvalidOperationException((string)task.Result[1]);
                    return ((string[])task.Result[2]).ToList();
                });
        }
    }
}
