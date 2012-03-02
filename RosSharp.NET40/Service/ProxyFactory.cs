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
    internal class ProxyFactory
    {
        private readonly MasterClient _masterClient;

        public ProxyFactory(MasterClient client)
        {
            _masterClient = client;
        }

        public Func<TRequest, IObservable<TResponse>> Create<TService, TRequest, TResponse>(string serviceName)
            where TService : IService<TRequest, TResponse>, new()
            where TRequest : IMessage, new()
            where TResponse : IMessage, new()
        {
            var ret1 = _masterClient
                .LookupServiceAsync("/test", serviceName).First();

            Console.WriteLine(ret1);


            var _tcpClient = new RosTcpClient();
            var ret = _tcpClient.ConnectAsync(ret1.Host, ret1.Port).First();

            var headerSerializer = new TcpRosHeaderSerializer<ServiceResponseHeader>();

            var rec = _tcpClient.ReceiveAsync()
                .Select(x => headerSerializer.Deserialize(new MemoryStream(x)))
                .Take(1)
                .PublishLast();

            rec.Connect();

            var service = new TService();

            var header = new ServiceHeader()
            {
                callerid = "test",
                md5sum = service.Md5Sum,
                service = serviceName  
            };

            var serializer = new TcpRosHeaderSerializer<ServiceHeader>();

            var stream = new MemoryStream();

            serializer.Serialize(stream, header);
            var data = stream.ToArray();

            _tcpClient.SendAsync(data).First();

            var test = rec.First();
            Console.WriteLine(test.callerid);

            return request => {

                var response = _tcpClient.ReceiveAsync(offset: 1)
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
                _tcpClient.SendAsync(senddata).First();

                return response;
            };
        }
    }
}
