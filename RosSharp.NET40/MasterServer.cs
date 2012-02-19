using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RosSharp
{
    public class MasterServer : MarshalByRefObject, IMaster
    {
        public override object InitializeLifetimeService()
        {
            return null;
        }

        public object[] RegisterService(string callerId, string service, string serviceApi, string callerApi)
        {
            throw new NotImplementedException();
        }

        public object[] UnregisterService(string callerId, string service, string serviceApi)
        {
            throw new NotImplementedException();
        }

        public object[] RegisterSubscriber(string callerId, string topic, string topicType, string callerApi)
        {
            throw new NotImplementedException();
        }

        public object[] UnregisterSubscriber(string callerId, string topic, string callerApi)
        {
            throw new NotImplementedException();
        }

        public object[] RegisterPublisher(string callerId, string topic, string topicType, string callerApi)
        {
            throw new NotImplementedException();
        }

        public object[] UnregisterPublisher(string callerId, string topic, string callerApi)
        {
            throw new NotImplementedException();
        }

        public object[] LookupNode(string callerId, string nodeName)
        {
            throw new NotImplementedException();
        }

        public object[] GetPublisherTopics(string callerId, string subgraph)
        {
            throw new NotImplementedException();
        }

        public object[] GetSystemState(string callerId)
        {
            throw new NotImplementedException();
        }

        public object[] GetMasterUri(string callerId)
        {
            throw new NotImplementedException();
        }

        public object[] LookupService(string callerId, string service)
        {
            throw new NotImplementedException();
        }
    }
}
