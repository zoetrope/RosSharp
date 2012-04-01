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

namespace RosSharp.Master
{
    /// <summary>
    ///   Asynchronous call Proxy for Master API
    /// </summary>
    [XmlRpcUrl("")]
    internal sealed class MasterProxy : XmlRpcClientProtocol
    {
        [XmlRpcBegin("registerService")]
        public IAsyncResult BeginRegisterService(string callerId, string service, string serviceApi, string callerApi, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] {callerId, service, serviceApi, callerApi}, callback, state);
        }

        [XmlRpcEnd]
        public object[] EndRegisterService(IAsyncResult result)
        {
            return (object[]) EndInvoke(result);
        }

        [XmlRpcBegin("unregisterService")]
        public IAsyncResult BeginUnregisterService(string callerId, string service, string serviceApi, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] {callerId, service, serviceApi}, callback, state);
        }

        [XmlRpcEnd]
        public object[] EndUnregisterService(IAsyncResult result)
        {
            return (object[]) EndInvoke(result);
        }

        [XmlRpcBegin("registerSubscriber")]
        public IAsyncResult BeginRegisterSubscriber(string callerId, string topic, string topicType, string callerApi, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] {callerId, topic, topicType, callerApi}, callback, state);
        }

        [XmlRpcEnd]
        public object[] EndRegisterSubscriber(IAsyncResult result)
        {
            return (object[]) EndInvoke(result);
        }

        [XmlRpcBegin("unregisterSubscriber")]
        public IAsyncResult BeginUnregisterSubscriber(string callerId, string topic, string callerApi, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] {callerId, topic, callerApi}, callback, state);
        }

        [XmlRpcEnd]
        public object[] EndUnregisterSubscriber(IAsyncResult result)
        {
            return (object[]) EndInvoke(result);
        }

        [XmlRpcBegin("registerPublisher")]
        public IAsyncResult BeginRegisterPublisher(string callerId, string topic, string topicType, string callerApi, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] {callerId, topic, topicType, callerApi}, callback, state);
        }

        [XmlRpcEnd]
        public object[] EndRegisterPublisher(IAsyncResult result)
        {
            return (object[]) EndInvoke(result);
        }

        [XmlRpcBegin("unregisterPublisher")]
        public IAsyncResult BeginUnregisterPublisher(string callerId, string topic, string callerApi, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] {callerId, topic, callerApi}, callback, state);
        }

        [XmlRpcEnd]
        public object[] EndUnregisterPublisher(IAsyncResult result)
        {
            return (object[]) EndInvoke(result);
        }

        [XmlRpcBegin("lookupNode")]
        public IAsyncResult BeginLookupNode(string callerId, string nodeName, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] {callerId, nodeName}, callback, state);
        }

        [XmlRpcEnd]
        public object[] EndLookupNode(IAsyncResult result)
        {
            return (object[]) EndInvoke(result);
        }

        [XmlRpcBegin("getPublisherTopics")]
        public IAsyncResult BeginGetPublisherTopics(string callerId, string subgraph, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] {callerId, subgraph}, callback, state);
        }

        [XmlRpcEnd]
        public object[] EndGetPublisherTopics(IAsyncResult result)
        {
            return (object[]) EndInvoke(result);
        }

        [XmlRpcBegin("getSystemState")]
        public IAsyncResult BeginGetSystemState(string callerId, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] {callerId}, callback, state);
        }

        [XmlRpcEnd]
        public object[] EndGetSystemState(IAsyncResult result)
        {
            return (object[]) EndInvoke(result);
        }

        [XmlRpcBegin("getUri")]
        public IAsyncResult BeginGetUri(string callerId, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] {callerId}, callback, state);
        }

        [XmlRpcEnd]
        public object[] EndGetUri(IAsyncResult result)
        {
            return (object[]) EndInvoke(result);
        }

        [XmlRpcBegin("lookupService")]
        public IAsyncResult BeginLookupService(string callerId, string service, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] {callerId, service}, callback, state);
        }

        [XmlRpcEnd]
        public object[] EndLookupService(IAsyncResult result)
        {
            return (object[]) EndInvoke(result);
        }
    }
}