using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using RosSharp.Master;
using RosSharp.Message;
using RosSharp.Transport;

namespace RosSharp.Service
{
    internal sealed class ServiceProxyFactory
    {
        public string NodeId { get; private set; }

        public ServiceProxyFactory(string nodeId)
        {
            NodeId = nodeId;
        }

        public TService Create<TService>(string serviceName, Uri uri) //TODO: 非同期にしなくては。
            where TService : IService, new()
        {
            var tcpClient = new RosTcpClient();
            tcpClient.ConnectTaskAsync(uri.Host, uri.Port).Wait();//TODO: waitではなくcontinueWith

            var rec = tcpClient.ReceiveAsync()
                .Select(x => TcpRosHeaderSerializer.Deserialize(new MemoryStream(x)))
                .Take(1)
                .PublishLast();

            rec.Connect();

            var service = new TService();

            var header = new 
            {
                callerid = NodeId,
                md5sum = service.Md5Sum,
                service = serviceName  
            };
            
            var stream = new MemoryStream();

            TcpRosHeaderSerializer.Serialize(stream, header);
            var data = stream.ToArray();

            tcpClient.SendTaskAsync(data).Wait();

            var test = rec.First();

            //TODO: proxyは返さない？どっかに持っておかなくてよい？
            var proxy = new ServiceProxy<TService>(service, tcpClient);

            return service;
        }

    }
}
