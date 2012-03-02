using CookComputing.XmlRpc;

namespace RosSharp.Parameter
{
    [XmlRpcUrl("")]
    public interface IParameterServer
    {
        [XmlRpcMethod("deleteParam")]
        object[] DeleteParam(string callerId, string key);
        [XmlRpcMethod("setParam")]
        object[] SetParam(string callerId, string key, object value);
        [XmlRpcMethod("getParam")]
        object[] GetParam(string callerId, string key);
        [XmlRpcMethod("searchParam")]
        object[] SearchParam(string callerId, string key);
        [XmlRpcMethod("subscribeParam")]
        object[] SubscribeParam(string callerId, string key, string callerApi);
        [XmlRpcMethod("unsubscribeParam")]
        object[] UnsubscribeParam(string callerId, string key, string callerApi);
        [XmlRpcMethod("hasParam")]
        object[] HasParam(string callerId, string key);
        [XmlRpcMethod("getParamNames")]
        object[] GetParamNames(string callerId);
    }
}