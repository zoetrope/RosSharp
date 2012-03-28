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
    internal class ServiceProxy<TService>
            where TService : IService, new()
    {
        private TService _service;
        RosTcpClient tcpClient;

        public ServiceProxy(TService service,RosTcpClient client)
        {
            tcpClient = client;
            _service = service;
            _service.SetAction(Invoke);
        }

        internal IMessage Invoke(IMessage request)
        {
            var response = tcpClient.ReceiveAsync(offset: 1)
                .Select(x =>
                {
                    //TODO: エラー処理
                    var res = _service.CreateResponse();
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
        }
    }
}
