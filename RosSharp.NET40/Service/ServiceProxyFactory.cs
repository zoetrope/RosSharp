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
    internal class ServiceProxyFactory
    {
        public string NodeId { get; private set; }

        public ServiceProxyFactory(string nodeId)
        {
            NodeId = nodeId;
        }

        public Func<TRequest, IObservable<TResponse>> Create<TService, TRequest, TResponse>(string serviceName, Uri uri)
            where TService : IService<TRequest, TResponse>, new()
            where TRequest : IMessage, new()
            where TResponse : IMessage, new()
        {

            var tcpClient = new RosTcpClient();
            var ret = tcpClient.ConnectAsync(uri.Host, uri.Port).First();

            var headerSerializer = new TcpRosHeaderSerializer<ServiceResponseHeader>();

            var rec = tcpClient.ReceiveAsync()
                .Select(x => headerSerializer.Deserialize(new MemoryStream(x)))
                .Take(1)
                .PublishLast();

            rec.Connect();

            var service = new TService();

            var header = new ServiceHeader()
            {
                callerid = NodeId,
                md5sum = service.Md5Sum,
                service = serviceName  
            };

            var serializer = new TcpRosHeaderSerializer<ServiceHeader>();

            var stream = new MemoryStream();

            serializer.Serialize(stream, header);
            var data = stream.ToArray();

            tcpClient.SendAsync(data).First();

            var test = rec.First();
            Console.WriteLine(test.callerid);

            return request => {

                var response = tcpClient.ReceiveAsync(offset: 1)
                    .Select(x =>
                    {
                        var res = new TResponse();
                        res.Deserialize(new MemoryStream(x));
                        return res;
                    })
                    .Take(1)
                    .PublishLast();

                response.Connect();

                var ms = new MemoryStream();
                request.Serialize(ms);
                var senddata = ms.ToArray();
                tcpClient.SendAsync(senddata).First();

                return response;
            };
        }
    }
}
