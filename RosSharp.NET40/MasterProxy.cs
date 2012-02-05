using System;
using System.Reflection;
using CookComputing.XmlRpc;

namespace RosSharp
{
    [XmlRpcUrl("")]
    public class MasterProxy : XmlRpcClientProtocol
    {

        [XmlRpcBegin("registerService")]
        public IAsyncResult BeginRegisterService(string callerId, string service, string serviceApi, string callerApi, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] { callerId, service, serviceApi, callerApi }, callback, state);
        }
        [XmlRpcEnd]
        public object[] EndRegisterService(IAsyncResult result)
        {
            return (object[])EndInvoke(result);
        }

        [XmlRpcBegin("unregisterService")]
        public IAsyncResult BeginUnregisterService(string callerId, string service, string serviceApi, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] { callerId, service, serviceApi }, callback, state);
        }
        [XmlRpcEnd]
        public object[] EndUnregisterService(IAsyncResult result)
        {
            return (object[])EndInvoke(result);
        }

        [XmlRpcBegin("registerSubscriber")]
        public IAsyncResult BeginRegisterSubscriber(string callerId, string topic, string topicType, string callerApi, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] { callerId, topic, topicType, callerApi}, callback, state);
        }
        [XmlRpcEnd]
        public object[] EndRegisterSubscriber(IAsyncResult result)
        {
            return (object[])EndInvoke(result);
        }

        [XmlRpcBegin("unregisterSubscriber")]
        public IAsyncResult BeginUnregisterSubscriber(string callerId, string topic, string callerApi, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] { callerId, topic, callerApi }, callback, state);
        }
        [XmlRpcEnd]
        public object[] EndUnregisterSubscriber(IAsyncResult result)
        {
            return (object[])EndInvoke(result);
        }

        [XmlRpcBegin("registerPublisher")]
        public IAsyncResult BeginRegisterPublisher(string callerId, string topic, string topicType, string callerApi, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] { callerId, topic, topicType, callerApi }, callback, state);
        }
        [XmlRpcEnd]
        public object[] EndRegisterPublisher(IAsyncResult result)
        {
            return (object[])EndInvoke(result);
        }

        [XmlRpcBegin("unregisterPublisher")]
        public IAsyncResult BeginUnregisterPublisher(string callerId, string topic, string callerApi, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] { callerId, topic, callerApi }, callback, state);
        }
        [XmlRpcEnd]
        public object[] EndUnregisterPublisher(IAsyncResult result)
        {
            return (object[])EndInvoke(result);
        }
        
        [XmlRpcBegin("lookupNode")]
        public IAsyncResult BeginLookupNode(string callerId, string nodeName, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] { callerId, nodeName }, callback, state);
        }
        [XmlRpcEnd]
        public object[] EndLookupNode(IAsyncResult result)
        {
            return (object[])EndInvoke(result);
        }

        [XmlRpcBegin("getPublisherTopics")]
        public IAsyncResult BeginGetPublisherTopics(string callerId, string subgraph, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] { callerId, subgraph }, callback, state);
        }
        [XmlRpcEnd]
        public object[] EndGetPublisherTopics(IAsyncResult result)
        {
            return (object[])EndInvoke(result);
        }

        [XmlRpcBegin("getSystemState")]
        public IAsyncResult BeginGetSystemState(string callerId, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] { callerId }, callback, state);
        }
        [XmlRpcEnd]
        public object[] EndGetSystemState(IAsyncResult result)
        {
            return (object[])EndInvoke(result);
        }

        [XmlRpcBegin("getUri")]
        public IAsyncResult BeginGetUri(string callerId, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] { callerId }, callback, state);
        }
        [XmlRpcEnd]
        public object[] EndGetUri(IAsyncResult result)
        {
            return (object[])EndInvoke(result);
        }

        [XmlRpcBegin("lookupService")]
        public IAsyncResult BeginLookupService(string callerId, string service, AsyncCallback callback, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] { callerId, service }, callback, state);
        }
        [XmlRpcEnd]
        public object[] EndLookupService(IAsyncResult result)
        {
            return (object[])EndInvoke(result);
        }

    }
}
