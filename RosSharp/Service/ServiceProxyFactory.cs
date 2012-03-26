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

        public TService Create<TService>(string serviceName, Uri uri)
            where TService : IService, new()
        {

            var tcpClient = new RosTcpClient();
            tcpClient.ConnectTaskAsync(uri.Host, uri.Port).Wait();

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


            service.SetAction(
                request =>
                {

                    var response = tcpClient.ReceiveAsync(offset: 1)
                        .Select(x =>
                                {
                                    //TODO: エラー処理
                                    var res = service.CreateResponse();
                                    var br = new BinaryReader(new MemoryStream(x));
                                    br.ReadInt32();
                                    res.Deserialize(br);
                                    return res;
                                })
                        .Take(1)
                        .PublishLast();

                    response.Connect();

                    var ms = new MemoryStream();
                    var bw = new BinaryWriter(ms);
                    bw.Write(request.SerializeLength);
                    request.Serialize(bw);
                    var senddata = ms.ToArray();
                    tcpClient.SendTaskAsync(senddata).Wait();

                    return response.First();
                });

            return service;
        }
    }
}
