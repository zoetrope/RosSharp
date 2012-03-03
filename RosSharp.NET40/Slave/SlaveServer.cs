using System;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using CookComputing.XmlRpc;
using RosSharp.Transport;

namespace RosSharp.Slave
{
    public class SlaveServer : MarshalByRefObject, ISlave
    {
        private RosTcpListener _listener;
        public Uri SlaveUri { get; set; }

        public SlaveServer(Uri uri)
        {
            _listener = new RosTcpListener();
            SlaveUri = uri;
        }

        public IObservable<Socket> AcceptAsync()
        {
            return _listener.AcceptAsync(0);
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
            //TODO: 固定値ではなくtopic名で判断
            var result = new object[3]
            {
                1,
                "Protocol<TCPROS, AdvertiseAddress<192.168.11.3, 8088>>",
                new object[3]{
                    "TCPROS",
                    "192.168.11.3",
                    _listener.Port
                }
            };

            return result;
        }
    }
}
