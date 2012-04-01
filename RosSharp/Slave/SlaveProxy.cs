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

namespace RosSharp.Slave
{
    /// <summary>
    ///   Asynchronous call Proxy for Slave API
    /// </summary>
    [XmlRpcUrl("")]
    internal sealed class SlaveProxy : XmlRpcClientProtocol
    {
        [XmlRpcBegin("getBusStats")]
        public IAsyncResult BeginGetBusStats(string callerId, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] {callerId}, callback, state);
        }

        [XmlRpcEnd]
        public object[] EndGetBusStats(IAsyncResult result)
        {
            return (object[]) EndInvoke(result);
        }

        [XmlRpcBegin("getBusInfo")]
        public IAsyncResult BeginGetBusInfo(string callerId, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] {callerId}, callback, state);
        }

        [XmlRpcEnd]
        public object[] EndGetBusInfo(IAsyncResult result)
        {
            return (object[]) EndInvoke(result);
        }

        [XmlRpcBegin("getMasterUri")]
        public IAsyncResult BeginGetMasterUri(string callerId, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] {callerId}, callback, state);
        }

        [XmlRpcEnd]
        public object[] EndGetMasterUri(IAsyncResult result)
        {
            return (object[]) EndInvoke(result);
        }

        [XmlRpcBegin("shutdown")]
        public IAsyncResult BeginShutdown(string callerId, string msg, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] {callerId, msg}, callback, state);
        }

        [XmlRpcEnd]
        public object[] EndShutdown(IAsyncResult result)
        {
            return (object[]) EndInvoke(result);
        }

        [XmlRpcBegin("getPid")]
        public IAsyncResult BeginGetPid(string callerId, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] {callerId}, callback, state);
        }

        [XmlRpcEnd]
        public object[] EndGetPid(IAsyncResult result)
        {
            return (object[]) EndInvoke(result);
        }

        [XmlRpcBegin("getSubscriptions")]
        public IAsyncResult BeginGetSubscriptions(string callerId, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] {callerId}, callback, state);
        }

        [XmlRpcEnd]
        public object[] EndGetSubscriptions(IAsyncResult result)
        {
            return (object[]) EndInvoke(result);
        }

        [XmlRpcBegin("getPublications")]
        public IAsyncResult BeginGetPublications(string callerId, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] {callerId}, callback, state);
        }

        [XmlRpcEnd]
        public object[] EndGetPublications(IAsyncResult result)
        {
            return (object[]) EndInvoke(result);
        }

        [XmlRpcBegin("paramUpdate")]
        public IAsyncResult BeginParamUpdate(string callerId, string parameterKey, object parameterValue, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] {callerId, parameterKey, parameterValue}, callback, state);
        }

        [XmlRpcEnd]
        public object[] EndParamUpdate(IAsyncResult result)
        {
            return (object[]) EndInvoke(result);
        }

        [XmlRpcBegin("publisherUpdate")]
        public IAsyncResult BeginPublisherUpdate(string callerId, string topic, string[] publishers, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] {callerId, topic, publishers}, callback, state);
        }

        [XmlRpcEnd]
        public object[] EndPublisherUpdate(IAsyncResult result)
        {
            return (object[]) EndInvoke(result);
        }

        [XmlRpcBegin("requestTopic")]
        public IAsyncResult BeginRequestTopic(string callerId, string topic, object[] protocols, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] {callerId, topic, protocols}, callback, state);
        }

        [XmlRpcEnd]
        public object[] EndRequestTopic(IAsyncResult result)
        {
            return (object[]) EndInvoke(result);
        }
    }
}