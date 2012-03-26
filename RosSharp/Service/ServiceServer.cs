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
    
    internal sealed class ServiceServer<TService>
        where TService : IService, new ()
    {

        private IService _service;

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
            client.ReceiveAsync()
                .Take(1)
                .Select(b => TcpRosHeaderSerializer.Deserialize(new MemoryStream(b)))
                .SelectMany(client.ReceiveAsync())
                .Subscribe(b =>
                           {
                               var res = Invoke(new MemoryStream(b));
                               var array = res.ToArray();
                               client.SendAsync(array).First();
                           });
                


            var dummy = new TService();
            var header = new 
            {
                callerid = _nodeId,
                md5sum = dummy.Md5Sum,
                service = serviceName,
                type = dummy.ServiceType
            };

            var ms = new MemoryStream();
            TcpRosHeaderSerializer.Serialize(ms, header);
            client.SendAsync(ms.ToArray()).First();
        }

        private MemoryStream Invoke(Stream stream)
        {
            var dummy = new TService();
            var req = dummy.CreateRequest();

            var br = new BinaryReader(stream);
            var len = br.ReadInt32();
            req.Deserialize(br);
            
            var res = _service.Invoke(req);

            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);
            bw.Write((byte) 1);
            bw.Write(res.SerializeLength);
            res.Serialize(bw);

            return ms;
        }
    }
}
