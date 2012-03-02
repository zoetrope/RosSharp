using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Text;
using CookComputing.XmlRpc;

namespace RosSharp
{
    public class RosNode : INode
    {
        private MasterClient _masterClient;
        private SlaveServer _slaveServer;
        public RosNode()
        {
            _masterClient = new MasterClient(new Uri("http://192.168.11.5:11311/"));


        }

        public Subscriber<TDataType> CreateSubscriber<TDataType>(string topicName) where TDataType : IMessage, new()
        {
            var ret1 = _masterClient
                .RegisterSubscriberAsync("/test", "chatter", "std_msgs/String", new Uri("http://192.168.11.2:11112"))
                .First();//TODO: エラーが起きたとき

            var slave = new SlaveClient(ret1.First());

            var topicParam = slave.RequestTopicAsync("/test", "/chatter", new object[1] { new string[1] { "TCPROS" } }).First();

            var subscriber = new Subscriber<TDataType>(topicParam);

            return subscriber;
        }
        
        public Publisher<TDataType> CreatePublisher<TDataType>(string topicName) where TDataType : IMessage, new()
        {
            _slaveServer = new SlaveServer();

            var publisher = new Publisher<TDataType>();

            _slaveServer.Connect().Subscribe(x => publisher.AddTopic(new RosTopic<TDataType>(x)));

            var channel = new HttpServerChannel("slave", 5678, new XmlRpcServerFormatterSinkProvider());
            ChannelServices.RegisterChannel(channel, false);
            RemotingServices.Marshal(_slaveServer, "slave");


            var ret1 = _masterClient.RegisterPublisherAsync("/test", "/chatter", "std_msgs/String", new Uri("http://192.168.11.4:5678/slave")).First();

            

            return publisher;
        }
        
        public Func<TRequest, TResponse> CreateProxy<TService, TRequest, TResponse>(string serviceName)
            where TService : IService<TRequest, TResponse>, new()
            where TRequest : IMessage, new()
            where TResponse : IMessage, new()
        {
            var ret1 = _masterClient
                .LookupServiceAsync("/test", serviceName).First();

            Console.WriteLine(ret1);


            var _tcpClient = new RosTcpClient();
            var ret = _tcpClient.ConnectAsObservable(ret1.Host, ret1.Port).First();

            var headerSerializer = new TcpRosHeaderSerializer<ServiceResponseHeader>();

            var rec = _tcpClient.ReceiveAsObservable()
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

            _tcpClient.SendAsObservable(data).First();

            var test = rec.First();
            Console.WriteLine(test.callerid);

            return request => {

                _tcpClient.ReceiveAsObservable(skip1Byte:true)
                    .Select(x => {
                        var res = new TResponse();
                        res.Deserialize(new MemoryStream(x));
                        return res;
                    })
                    .Take(1)
                    .Subscribe(x => Console.WriteLine(x));

                var ms = new MemoryStream();
                request.Serialize(ms);
                var senddata = ms.ToArray();
                _tcpClient.SendAsObservable(senddata).First();

                return new TResponse(); 
            };
        }
        
    }
}
