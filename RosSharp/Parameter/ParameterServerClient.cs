using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

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
        public IObservable<Unit> DeleteParamAsync(string callerId, string key)
        {
            return Observable.FromAsyncPattern<string, string, object[]>(_proxy.BeginDeleteParam, _proxy.EndDeleteParam)
                .Invoke(callerId, key)
                .Do(ret => { if ((int) ret[0] != 1) throw new InvalidOperationException((string) ret[1]); })
                .Select(ret => Unit.Default);
        }

        /// <summary>
        /// Set parameter.
        /// </summary>
        /// <param name="callerId">ROS caller ID</param>
        /// <param name="key">Parameter name.</param>
        /// <param name="value">Parameter value.</param>
        /// <returns>ignore</returns>
        public IObservable<Unit> SetParamAsync(string callerId, string key, object value)
        {
#if WINDOWS_PHONE
            return ObservableEx
#else
            return Observable
#endif
                .FromAsyncPattern<string, string, object, object[]>(_proxy.BeginSetParam, _proxy.EndSetParam)
                .Invoke(callerId, key, value)
                .Do(ret => { if ((int)ret[0] != 1) throw new InvalidOperationException((string)ret[1]); })
                .Select(ret => Unit.Default);
        }

        /// <summary>
        /// Retrieve parameter value from server.
        /// </summary>
        /// <param name="callerId">ROS caller ID</param>
        /// <param name="key">Parameter name. If key is a namespace, getParam() will return a parameter tree.</param>
        /// <returns>parameterValue</returns>
        public IObservable<object[]> GetParamAsync(string callerId, string key)
        {
            return Observable.FromAsyncPattern<string, string, object[]>(_proxy.BeginGetParam, _proxy.EndGetParam)
                .Invoke(callerId, key)
                .Do(ret => { if ((int)ret[0] != 1) throw new InvalidOperationException((string)ret[1]); });
        }

        /// <summary>
        /// Search for parameter key on the Parameter Server.
        /// </summary>
        /// <param name="callerId">ROS caller ID</param>
        /// <param name="key">Parameter name to search for.</param>
        /// <returns>foundKey</returns>
        public IObservable<string> SearchParamAsync(string callerId, string key)
        {
            return Observable.FromAsyncPattern<string, string, object[]>(_proxy.BeginSearchParam, _proxy.EndSearchParam)
                .Invoke(callerId, key)
                .Do(ret => { if ((int)ret[0] != 1) throw new InvalidOperationException((string)ret[1]); })
                .Select(ret => (string)ret[2]);
        }

        /// <summary>
        /// Retrieve parameter value from server and subscribe to updates to that param.
        /// See paramUpdate() in the Node API.
        /// </summary>
        /// <param name="callerId">ROS caller ID.</param>
        /// <param name="callerApi">Node API URI of subscriber for paramUpdate callbacks.</param>
        /// <param name="key">Parameter name</param>
        /// <returns>parameterValue</returns>
        public IObservable<object[]> SubscribeParamAsync(string callerId, Uri callerApi, string key)
        {
#if WINDOWS_PHONE
            return ObservableEx
#else
            return Observable
#endif
                .FromAsyncPattern<string, string, string, object[]>(_proxy.BeginSubscribeParam, _proxy.EndSubscribeParam)
                .Invoke(callerId, callerApi.ToString(), key)
                .Do(ret => { if ((int)ret[0] != 1) throw new InvalidOperationException((string)ret[1]); });
        }

        /// <summary>
        /// Retrieve parameter value from server and subscribe to updates to that param. 
        /// See paramUpdate() in the Node API.
        /// </summary>
        /// <param name="callerId">ROS caller ID.</param>
        /// <param name="callerApi">Node API URI of subscriber.</param>
        /// <param name="key">Parameter name.</param>
        /// <returns>number of unsubscribed</returns>
        public IObservable<int> UnsubscribeParamAsync(string callerId, Uri callerApi, string key)
        {
#if WINDOWS_PHONE
            return ObservableEx
#else
            return Observable
#endif
                .FromAsyncPattern<string, string, string, object[]>(_proxy.BeginUnsubscribeParam, _proxy.EndUnsubscribeParam)
                .Invoke(callerId, callerApi.ToString(), key)
                .Do(ret => { if ((int)ret[0] != 1) throw new InvalidOperationException((string)ret[1]); })
                .Select(ret => (int)ret[2]);
        }

        /// <summary>
        /// Check if parameter is stored on server.
        /// </summary>
        /// <param name="callerId">ROS caller ID.</param>
        /// <param name="key">Parameter name.</param>
        /// <returns>hasParam</returns>
        public IObservable<bool> HasParamAsync(string callerId, string key)
        {
            return Observable.FromAsyncPattern<string, string, object[]>(_proxy.BeginHasParam, _proxy.EndHasParam)
                .Invoke(callerId, key)
                .Do(ret => { if ((int)ret[0] != 1) throw new InvalidOperationException((string)ret[1]); })
                .Select(ret => (bool)ret[2]);
        }

        /// <summary>
        /// Get list of all parameter names stored on this server.
        /// </summary>
        /// <param name="callerId">ROS caller ID.</param>
        /// <returns>parameter name list</returns>
        public IObservable<List<string>> GetParamNamesAsync(string callerId)
        {
            return Observable.FromAsyncPattern<string, object[]>(_proxy.BeginGetParamNames, _proxy.EndGetParamNames)
                .Invoke(callerId)
                .Do(ret => { if ((int)ret[0] != 1) throw new InvalidOperationException((string)ret[1]); })
                .Select(ret => ((string[])ret[2]).ToList());
        }
    }
}
