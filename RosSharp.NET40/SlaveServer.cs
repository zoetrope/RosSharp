using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RosSharp
{
    public class SlaveServer : MarshalByRefObject, ISlave
    {
        public object[] GetBusStats(string callerId)
        {
            throw new NotImplementedException();
        }

        public object[] GetBusInfo(string callerId)
        {
            throw new NotImplementedException();
        }

        public object[] GetMasterUri(string callerId)
        {
            throw new NotImplementedException();
        }

        public object[] Shutdown(string callerId, string msg)
        {
            throw new NotImplementedException();
        }

        public object[] GetPid(string callerId)
        {
            throw new NotImplementedException();
        }

        public object[] GetSubscriptions(string callerId)
        {
            throw new NotImplementedException();
        }

        public object[] GetPublications(string callerId)
        {
            throw new NotImplementedException();
        }

        public object[] ParamUpdate(string callerId, string parameterKey, object parameterValue)
        {
            throw new NotImplementedException();
        }

        public object[] PublisherUpdate(string callerId, string topic, string[] publishers)
        {
            throw new NotImplementedException();
        }

        public object[] RequestTopic(string callerId, string topic, object[] protocols)
        {
            throw new NotImplementedException();
        }
    }
}
