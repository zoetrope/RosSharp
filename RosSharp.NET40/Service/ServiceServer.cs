using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Text;
using RosSharp.Message;
using RosSharp.Transport;

namespace RosSharp.Service
{
    
    public class ServiceServer<TService, TRequest, TResponse>
        where TService : IService<TRequest, TResponse>, new()
        where TRequest : IMessage, new()
        where TResponse : IMessage, new()
    {

        private Func<TRequest, TResponse> _service;

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

        public IDisposable RegisterService(string serviceName, Func<TRequest, TResponse> service)
        {
            _service = service;

            _listener = new RosTcpListener(0);
            var disp = _listener.AcceptAsync()
                .Select(s => new RosTcpClient(s))
                .Subscribe(client => Initialize(serviceName, client));
                
            //TODO: Accept用のポート番号が割当たる前にRegisterServiceしてしまう？？ListenしてるからOKか。

            return disp;//TODO: サービス登録を解除するためのDisposableを返す。

        }

        private void Initialize(string serviceName, RosTcpClient client)
        {
            var headerSerializer =
                new TcpRosHeaderSerializer<ServiceResponseHeader>();
            client.ReceiveAsync()
                .Take(1)
                .Select(b => headerSerializer.Deserialize(new MemoryStream(b)))
                .SelectMany(client.ReceiveAsync())
                .Subscribe(b =>
                           {
                               var stream = new MemoryStream(b);
                               var res = Invoke(stream);
                               var array = res.ToArray();
                               client.SendAsync(array).First();
                           });
                


            var dummy = new TService();
            var header = new ServiceResponseHeader()
            {
                callerid = _nodeId,
                md5sum = dummy.Md5Sum,
                service = serviceName,
                type = dummy.ServiceType
            };

            var ms = new MemoryStream();
            headerSerializer.Serialize(ms, header);
            client.SendAsync(ms.ToArray()).First();
        }

        private MemoryStream Invoke(Stream stream)
        {
            var req = new TRequest();
            req.Deserialize(stream);
            var res = _service(req);
            var ms = new MemoryStream();
            res.Serialize(ms);
            return ms;
        }
    }
}
