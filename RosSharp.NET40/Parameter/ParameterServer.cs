using System;

namespace RosSharp.Parameter
{
    public class ParameterServer : MarshalByRefObject, IParameterServer
    {
        public override object InitializeLifetimeService()
        {
            return null;
        }

        public object[] DeleteParam(string callerId, string key)
        {
            throw new NotImplementedException();
        }

        public object[] SetParam(string callerId, string key, object value)
        {
            throw new NotImplementedException();
        }

        public object[] GetParam(string callerId, string key)
        {
            throw new NotImplementedException();
        }

        public object[] SearchParam(string callerId, string key)
        {
            throw new NotImplementedException();
        }

        public object[] SubscribeParam(string callerId, string key, string callerApi)
        {
            throw new NotImplementedException();
        }

        public object[] UnsubscribeParam(string callerId, string key, string callerApi)
        {
            throw new NotImplementedException();
        }

        public object[] HasParam(string callerId, string key)
        {
            throw new NotImplementedException();
        }

        public object[] GetParamNames(string callerId)
        {
            throw new NotImplementedException();
        }
    }
}
