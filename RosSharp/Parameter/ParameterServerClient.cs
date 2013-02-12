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
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace RosSharp.Parameter
{
    /// <summary>
    ///   XML-RPC Client for ParameterServer API
    /// </summary>
    public sealed class ParameterServerClient
    {
        private ParameterServerProxy _proxy;

        public ParameterServerClient(Uri uri)
        {
            _proxy = new ParameterServerProxy();
            _proxy.Url = uri.ToString();
            _proxy.Timeout = Ros.XmlRpcTimeout;
        }

        /// <summary>
        ///   Delete parameter
        /// </summary>
        /// <param name="callerId"> ROS caller ID </param>
        /// <param name="key"> Parameter name. </param>
        /// <returns> ignore </returns>
        public async Task DeleteParamAsync(string callerId, string key)
        {
            var result = await Task<object[]>.Factory.FromAsync(_proxy.BeginDeleteParam, _proxy.EndDeleteParam, callerId, key, null);
            if ((StatusCode)result[0] != StatusCode.Success) throw new InvalidOperationException((string)result[1]);
        }

        /// <summary>
        ///   Set parameter.
        /// </summary>
        /// <param name="callerId"> ROS caller ID </param>
        /// <param name="key"> Parameter name. </param>
        /// <param name="value"> Parameter value. </param>
        /// <returns> ignore </returns>
        public async Task SetParamAsync(string callerId, string key, object value)
        {
            var result = await Task<object[]>.Factory.FromAsync(_proxy.BeginSetParam, _proxy.EndSetParam, callerId, key, value, null);
            if ((StatusCode)result[0] != StatusCode.Success) throw new InvalidOperationException((string)result[1]);
        }

        /// <summary>
        ///   Retrieve parameter value from server.
        /// </summary>
        /// <param name="callerId"> ROS caller ID </param>
        /// <param name="key"> Parameter name. If key is a namespace, getParam() will return a parameter tree. </param>
        /// <returns> parameterValue </returns>
        public async Task<object> GetParamAsync(string callerId, string key)
        {
            var result = await Task<object[]>.Factory.FromAsync(_proxy.BeginGetParam, _proxy.EndGetParam, callerId, key, null);
            if ((StatusCode)result[0] != StatusCode.Success) throw new InvalidOperationException((string)result[1]);
            return result[2];
        }

        /// <summary>
        ///   Search for parameter key on the Parameter Server.
        /// </summary>
        /// <param name="callerId"> ROS caller ID </param>
        /// <param name="key"> Parameter name to search for. </param>
        /// <returns> foundKey </returns>
        public async Task<string> SearchParamAsync(string callerId, string key)
        {
            var result = await Task<object[]>.Factory.FromAsync(_proxy.BeginSearchParam, _proxy.EndSearchParam, callerId, key, null);
            if ((StatusCode)result[0] != StatusCode.Success) throw new InvalidOperationException((string)result[1]);
            return (string)result[2];
        }

        /// <summary>
        ///   Retrieve parameter value from server and subscribe to updates to that param. See paramUpdate() in the Node API.
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <param name="callerApi"> Node API URI of subscriber for paramUpdate callbacks. </param>
        /// <param name="key"> Parameter name </param>
        /// <returns> parameterValue </returns>
        public async Task<object> SubscribeParamAsync(string callerId, Uri callerApi, string key)
        {
            var result = await Task<object[]>.Factory.FromAsync(_proxy.BeginSubscribeParam, _proxy.EndSubscribeParam, callerId, callerApi.ToString(), key, null);
            if ((StatusCode)result[0] != StatusCode.Success) throw new InvalidOperationException((string)result[1]);
            return result[2];
        }

        /// <summary>
        ///   Retrieve parameter value from server and subscribe to updates to that param. See paramUpdate() in the Node API.
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <param name="callerApi"> Node API URI of subscriber. </param>
        /// <param name="key"> Parameter name. </param>
        /// <returns> number of unsubscribed </returns>
        public async Task<int> UnsubscribeParamAsync(string callerId, Uri callerApi, string key)
        {
            var result = await Task<object[]>.Factory.FromAsync(_proxy.BeginUnsubscribeParam, _proxy.EndUnsubscribeParam, callerId, callerApi.ToString(), key, null);
            if ((StatusCode)result[0] != StatusCode.Success) throw new InvalidOperationException((string)result[1]);
            return (int)result[2];
        }

        /// <summary>
        ///   Check if parameter is stored on server.
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <param name="key"> Parameter name. </param>
        /// <returns> hasParam </returns>
        public async Task<bool> HasParamAsync(string callerId, string key)
        {
            var result = await Task<object[]>.Factory.FromAsync(_proxy.BeginHasParam, _proxy.EndHasParam, callerId, key, null);
            if ((StatusCode)result[0] != StatusCode.Success) throw new InvalidOperationException((string)result[1]);
            return (bool)result[2];
        }

        /// <summary>
        ///   Get list of all parameter names stored on this server.
        /// </summary>
        /// <param name="callerId"> ROS caller ID. </param>
        /// <returns> parameter name list </returns>
        public async Task<List<string>> GetParamNamesAsync(string callerId)
        {
            var result = await Task<object[]>.Factory.FromAsync(_proxy.BeginGetParamNames, _proxy.EndGetParamNames, callerId, null);
            if ((StatusCode)result[0] != StatusCode.Success) throw new InvalidOperationException((string)result[1]);
            return ((string[])result[2]).ToList();
        }
    }
}