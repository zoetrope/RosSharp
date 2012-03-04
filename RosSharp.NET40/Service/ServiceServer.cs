using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public int Port
        {
            get { return _listener.Port; }
        }

        public ServiceServer(string nodeId)
        {
            _nodeId = nodeId;
        }

        public IDisposable RegisterService(string serviceName, Func<TRequest, TResponse> service)
        {
            _service = service;

            _listener = new RosTcpListener();
            var disp = _listener.AcceptAsync(0)
                .Select(s => new RosTcpClient(s))
                //.SelectMany(c => c.ReceiveAsync())
                //.Subscribe(b =>
                .Subscribe(client => client.ReceiveAsync().Subscribe(b =>
                {
                    if (b.Length == 20)//TODO:これはひどい
                    {
                        //TODO: 先にヘッダを処理しないと。
                        var ms = new MemoryStream(b);
                        var res = Invoke(ms);
                        var array = res.ToArray();
                        client.SendAsync(array).First();
                    }
                    else
                    {
                        var dummy = new TService();
                        var header = new ServiceResponseHeader()
                        {
                            callerid = _nodeId,
                            md5sum = dummy.Md5Sum,
                            service = serviceName,
                            type = dummy.ServiceType
                        };

                        var headerSerializer = new TcpRosHeaderSerializer<ServiceResponseHeader>();
                        var ms = new MemoryStream();
                        headerSerializer.Serialize(ms, header);
                        client.SendAsync(ms.ToArray()).First();

                    }
                }));

            //TODO: Accept用のポート番号が割当たる前にRegisterServiceしてしまう？？ListenしてるからOKか。

            return disp;//TODO: サービス登録を解除するためのDisposableを返す。

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
