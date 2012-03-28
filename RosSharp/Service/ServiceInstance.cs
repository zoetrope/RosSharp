using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Text;
using RosSharp.Transport;

namespace RosSharp.Service
{
    public class ServiceInstance<TService>
        where TService : IService, new()
    {
        private IService _service;
        private string _nodeId;
        private RosTcpClient client;

        public ServiceInstance(string nodeId, IService service, Socket s)
        {
            _nodeId = nodeId;
            _service = service;

            client = new RosTcpClient(s);
        }

        internal void Initialize(string serviceName)
        {
            client.ReceiveAsync()
                .Take(1)
                .Select(b => TcpRosHeaderSerializer.Deserialize(new MemoryStream(b)))
                .SelectMany(client.ReceiveAsync())
                .Subscribe(b =>
                {
                    var res = Invoke(new MemoryStream(b));
                    var array = res.ToArray();
                    client.SendTaskAsync(array).Wait();//TODO: Waitしても意味なくね？Subscribe自体の終了を待たねば。
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
            client.SendTaskAsync(ms.ToArray()).Wait();
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
            bw.Write((byte)1);
            bw.Write(res.SerializeLength);
            res.Serialize(bw);

            return ms;
        }
    }
}
