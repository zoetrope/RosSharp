using System;
using System.Reflection;
using CookComputing.XmlRpc;

namespace RosSharp
{
    [XmlRpcUrl("")]
    public class SlaveProxy : XmlRpcClientProtocol
    {
        [XmlRpcBegin("getBusStats")]
        public IAsyncResult BeginGetBusStats(string callerId, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] { callerId }, callback, state);
        }
        [XmlRpcEnd]
        public object[] EndGetBusStats(IAsyncResult result)
        {
            return (object[])EndInvoke(result);
        }

        [XmlRpcBegin("getBusInfo")]
        public IAsyncResult BeginGetBusInfo(string callerId, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] { callerId }, callback, state);
        }
        [XmlRpcEnd]
        public object[] EndGetBusInfo(IAsyncResult result)
        {
            return (object[])EndInvoke(result);
        }
        
        [XmlRpcBegin("getMasterUri")]
        public IAsyncResult BeginGetMasterUri(string callerId, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] { callerId }, callback, state);
        }
        [XmlRpcEnd]
        public object[] EndGetMasterUri(IAsyncResult result)
        {
            return (object[])EndInvoke(result);
        }
        
        [XmlRpcBegin("shutdown")]
        public IAsyncResult BeginShutdown(string callerId, string msg, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] { callerId, msg }, callback, state);
        }
        [XmlRpcEnd]
        public object[] EndShutdown(IAsyncResult result)
        {
            return (object[])EndInvoke(result);
        }
        
        [XmlRpcBegin("getPid")]
        public IAsyncResult BeginGetPid(string callerId, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] { callerId }, callback, state);
        }
        [XmlRpcEnd]
        public object[] EndGetPid(IAsyncResult result)
        {
            return (object[])EndInvoke(result);
        }
        
        [XmlRpcBegin("getSubscriptions")]
        public IAsyncResult BeginGetSubscriptions(string callerId, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] { callerId }, callback, state);
        }
        [XmlRpcEnd]
        public object[] EndGetSubscriptions(IAsyncResult result)
        {
            return (object[])EndInvoke(result);
        }
        
        [XmlRpcBegin("getPublications")]
        public IAsyncResult BeginGetPublications(string callerId, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] { callerId }, callback, state);
        }
        [XmlRpcEnd]
        public object[] EndGetPublications(IAsyncResult result)
        {
            return (object[])EndInvoke(result);
        }
        
        [XmlRpcBegin("paramUpdate")]
        public IAsyncResult BeginParamUpdate(string callerId, string parameterKey, object parameterValue, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] { callerId, parameterKey, parameterValue }, callback, state);
        }
        [XmlRpcEnd]
        public object[] EndParamUpdate(IAsyncResult result)
        {
            return (object[])EndInvoke(result);
        }
        
        [XmlRpcBegin("publisherUpdate")]
        public IAsyncResult BeginPublisherUpdate(string callerId, string topic, string[] publishers, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] { callerId, topic, publishers }, callback, state);
        }
        [XmlRpcEnd]
        public object[] EndPublisherUpdate(IAsyncResult result)
        {
            return (object[])EndInvoke(result);
        }
        
        [XmlRpcBegin("requestTopic")]
        public IAsyncResult BeginRequestTopic(string callerId, string topic, object[] protocols, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] { callerId, topic, protocols }, callback, state);
        }
        [XmlRpcEnd]
        public object[] EndRequestTopic(IAsyncResult result)
        {
            return (object[])EndInvoke(result);
        }
    }
}