using CookComputing.XmlRpc;

namespace RosSharp
{
    [XmlRpcUrl("")]
    public interface IMaster
    {
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
    }
}
