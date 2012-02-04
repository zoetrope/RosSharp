using CookComputing.XmlRpc;

namespace RosSharp
{
    [XmlRpcUrl("")]
    public interface ISlave : IXmlRpcProxy
    {
        [XmlRpcMethod("getBusStats")]
        object[] GetBusStats(string callerId);
        [XmlRpcMethod("getBusInfo")]
        object[] GetBusInfo(string callerId);
        [XmlRpcMethod("getMasterUri")]
        object[] GetMasterUri(string callerId);
        [XmlRpcMethod("shutdown")]
        object[] Shutdown(string callerId, string msg);
        [XmlRpcMethod("getPid")]
        object[] GetPid(string callerId);
        [XmlRpcMethod("getSubscriptions")]
        object[] GetSubscriptions(string callerId);
        [XmlRpcMethod("getPublications")]
        object[] GetPublications(string callerId);
        [XmlRpcMethod("paramUpdate")]
        object[] ParamUpdate(string callerId, string parameterKey, object parameterValue);
        [XmlRpcMethod("publisherUpdate")]
        object[] PublisherUpdate(string callerId, string topic, string[] publishers);
        [XmlRpcMethod("requestTopic")]
        object[] RequestTopic(string callerId, string topic, object[] protocols);
    }
}