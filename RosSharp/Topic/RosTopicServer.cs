using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using RosSharp.Transport;
using System.Net.Sockets;

namespace RosSharp.Topic
{
    public sealed class RosTopicServer
    {
        private readonly RosTcpListener _listener;

        public RosTopicServer()
        {
            _listener = new RosTcpListener(0);
        }

        
        public IPEndPoint AdvertiseAddress
        {
            get { return _listener.EndPoint; }
        }

        public IObservable<Socket> AcceptAsync()
        {
            return _listener.AcceptAsync();
        }

    }
}
