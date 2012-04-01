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
using System.Reflection;
using CookComputing.XmlRpc;

namespace RosSharp.Parameter
{
    /// <summary>
    ///   Asynchronous call Proxy for ParameterServer API
    /// </summary>
    [XmlRpcUrl("")]
    internal sealed class ParameterServerProxy : XmlRpcClientProtocol
    {
        [XmlRpcBegin("deleteParam")]
        public IAsyncResult BeginDeleteParam(string callerId, string key, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] {callerId, key}, callback, state);
        }

        [XmlRpcEnd]
        public object[] EndDeleteParam(IAsyncResult result)
        {
            return (object[]) EndInvoke(result);
        }

        [XmlRpcBegin("setParam")]
        public IAsyncResult BeginSetParam(string callerId, string key, object value, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] {callerId, key, value}, callback, state);
        }

        [XmlRpcEnd]
        public object[] EndSetParam(IAsyncResult result)
        {
            return (object[]) EndInvoke(result);
        }

        [XmlRpcBegin("getParam")]
        public IAsyncResult BeginGetParam(string callerId, string key, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] {callerId, key}, callback, state);
        }

        [XmlRpcEnd]
        public object[] EndGetParam(IAsyncResult result)
        {
            return (object[]) EndInvoke(result);
        }

        [XmlRpcBegin("searchParam")]
        public IAsyncResult BeginSearchParam(string callerId, string key, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] {callerId, key}, callback, state);
        }

        [XmlRpcEnd]
        public object[] EndSearchParam(IAsyncResult result)
        {
            return (object[]) EndInvoke(result);
        }

        [XmlRpcBegin("subscribeParam")]
        public IAsyncResult BeginSubscribeParam(string callerId, string callerApi, string key, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] {callerId, callerApi, key}, callback, state);
        }

        [XmlRpcEnd]
        public object[] EndSubscribeParam(IAsyncResult result)
        {
            return (object[]) EndInvoke(result);
        }

        [XmlRpcBegin("unsubscribeParam")]
        public IAsyncResult BeginUnsubscribeParam(string callerId, string callerApi, string key, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] {callerId, callerApi, key}, callback, state);
        }

        [XmlRpcEnd]
        public object[] EndUnsubscribeParam(IAsyncResult result)
        {
            return (object[]) EndInvoke(result);
        }

        [XmlRpcBegin("hasParam")]
        public IAsyncResult BeginHasParam(string callerId, string key, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] {callerId, key}, callback, state);
        }

        [XmlRpcEnd]
        public object[] EndHasParam(IAsyncResult result)
        {
            return (object[]) EndInvoke(result);
        }

        [XmlRpcBegin("getParamNames")]
        public IAsyncResult BeginGetParamNames(string callerId, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] {callerId}, callback, state);
        }

        [XmlRpcEnd]
        public object[] EndGetParamNames(IAsyncResult result)
        {
            return (object[]) EndInvoke(result);
        }
    }
}