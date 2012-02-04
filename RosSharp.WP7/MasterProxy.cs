using System;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using CookComputing.XmlRpc;

namespace RosSharp
{
    [XmlRpcUrl("")]
    public class MasterProxy : XmlRpcClientProtocol
    {
        [XmlRpcBegin("lookupNode")]
        public IAsyncResult BeginLookupNode(string callerId, string nodeName, AsyncCallback acb, object state)
        {
            return BeginInvoke(MethodBase.GetCurrentMethod(), new object[] { callerId, nodeName }, acb, state);
        }

        [XmlRpcEnd]
        public object[] EndLookupNode(IAsyncResult iasr)
        {
            var ret = (object[]) EndInvoke(iasr);
            return ret;
        }


        /*
        [XmlRpcMethod("registerService")]
        object[] RegisterService(string callerId, string service, string serviceApi, string callerApi);
        [XmlRpcMethod("unregisterService")]
        object[] UnregisterService(string callerId, string service, string serviceApi);
        [XmlRpcMethod("registerSubscriber")]
        object[] RegisterSubscriber(string callerId, string topic, string topicType, string callerApi);
        [XmlRpcMethod("unregisterSubscriber")]
        object[] UnregisterSubscriber(string callerId, string topic, string callerApi);
        [XmlRpcMethod("registerPublisher")]
        object[] RegisterPublisher(string callerId, string topic, string topicType, string callerApi);
        [XmlRpcMethod("unregisterPublisher")]
        object[] UnregisterPublisher(string callerId, string topic, string callerApi);
        [XmlRpcMethod("lookupNode")]
        object[] LookupNode(string callerId, string nodeName);
        [XmlRpcMethod("getPublisherTopics")]
        object[] GetPublisherTopics(string callerId, string subgraph);
        [XmlRpcMethod("getSystemState")]
        object[] GetSystemState(string callerId);
        [XmlRpcMethod("getMasterUri")]
        object[] GetMasterUri(string callerId);
        [XmlRpcMethod("lookupService")]
        object[] LookupService(string callerId, string service);
        */
    }
}
