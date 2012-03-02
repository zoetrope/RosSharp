using System;
using System.Net.Sockets;
using RosSharp.Transport;

namespace RosSharp.Slave
{
    public class SlaveServer : MarshalByRefObject, ISlave
    {
        private RosTcpListener _listener;
        
        public SlaveServer()
        {
            _listener = new RosTcpListener();
            
            
        }

        public IObservable<Socket> AcceptAsync()
        {
            return _listener.AcceptAsync(8088);
        }


        public override object InitializeLifetimeService()
        {
            return null;
        }

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

            var result = new object[3]
            {
                1,
                "Protocol<TCPROS, AdvertiseAddress<192.168.11.4, 8088>>",
                new object[3]{
                    "TCPROS",
                    "192.168.11.4",
                    8088
                }
            };

            return result;
        }
    }
}
