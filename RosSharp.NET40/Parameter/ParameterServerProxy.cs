using System;
using System.Reflection;
using CookComputing.XmlRpc;

namespace RosSharp.Parameter
{
    /// <summary>
    /// Asynchronous call Proxy for ParameterServer API
    /// </summary>
    [XmlRpcUrl("")]
    internal sealed class ParameterServerProxy : XmlRpcClientProtocol
    {
        [XmlRpcBegin("deleteParam")]
        public IAsyncResult BeginDeleteParam(string callerId, string key, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] { callerId, key }, callback, state);
        }
        [XmlRpcEnd]
        public object[] EndDeleteParam(IAsyncResult result)
        {
            return (object[])EndInvoke(result);
        }

        [XmlRpcBegin("setParam")]
        public IAsyncResult BeginSetParam(string callerId, string key, object value, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] { callerId, key, value }, callback, state);
        }
        [XmlRpcEnd]
        public object[] EndSetParam(IAsyncResult result)
        {
            return (object[])EndInvoke(result);
        }

        [XmlRpcBegin("getParam")]
        public IAsyncResult BeginGetParam(string callerId, string key, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] { callerId, key }, callback, state);
        }
        [XmlRpcEnd]
        public object[] EndGetParam(IAsyncResult result)
        {
            return (object[])EndInvoke(result);
        }

        [XmlRpcBegin("searchParam")]
        public IAsyncResult BeginSearchParam(string callerId, string key, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] { callerId, key }, callback, state);
        }
        [XmlRpcEnd]
        public object[] EndSearchParam(IAsyncResult result)
        {
            return (object[])EndInvoke(result);
        }

        [XmlRpcBegin("subscribeParam")]
        public IAsyncResult BeginSubscribeParam(string callerId, string key, string callerApi, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] { callerId, key, callerApi }, callback, state);
        }
        [XmlRpcEnd]
        public object[] EndSubscribeParam(IAsyncResult result)
        {
            return (object[])EndInvoke(result);
        }

        [XmlRpcBegin("unsubscribeParam")]
        public IAsyncResult BeginUnsubscribeParam(string callerId, string key, string callerApi, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] { callerId, key, callerApi }, callback, state);
        }
        [XmlRpcEnd]
        public object[] EndUnsubscribeParam(IAsyncResult result)
        {
            return (object[])EndInvoke(result);
        }

        [XmlRpcBegin("hasParam")]
        public IAsyncResult BeginHasParam(string callerId, string key, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] { callerId, key }, callback, state);
        }
        [XmlRpcEnd]
        public object[] EndHasParam(IAsyncResult result)
        {
            return (object[])EndInvoke(result);
        }

        [XmlRpcBegin("getParamNames")]
        public IAsyncResult BeginGetParamNames(string callerId, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] { callerId }, callback, state);
        }
        [XmlRpcEnd]
        public object[] EndGetParamNames(IAsyncResult result)
        {
            return (object[])EndInvoke(result);
        }
    }
}