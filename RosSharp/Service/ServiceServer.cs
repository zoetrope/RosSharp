using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Text;
using RosSharp.Message;
using RosSharp.Transport;

namespace RosSharp.Service
{
    
    internal sealed class ServiceServer<TService>
        where TService : IService, new ()
    {
        private string _nodeId;
        private RosTcpListener _listener;

        public IPEndPoint EndPoint
        {
            get { return _listener.EndPoint; }
        }

        public ServiceServer(string nodeId)
        {
            _nodeId = nodeId;
        }

        public IDisposable RegisterService(string serviceName, IService service)
        {
            _listener = new RosTcpListener(0);
            var disp = _listener.AcceptAsync()
                .Select(s => new ServiceInstance<TService>(_nodeId,service, s))
                .Subscribe(client => client.Initialize(serviceName));
                
            return disp;//TODO: サービス登録を解除するためのDisposableを返す。もしくはIObservable？
        }

    }

}
