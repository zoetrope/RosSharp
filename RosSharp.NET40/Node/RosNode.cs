using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using CookComputing.XmlRpc;
using RosSharp.Master;
using RosSharp.Message;
using RosSharp.Service;
using RosSharp.Slave;
using RosSharp.Topic;
using RosSharp.Transport;

namespace RosSharp.Node
{
    public class RosNode : INode
    {
        private MasterClient _masterClient { get; set; }
        //private SlaveServer _slaveServer;

        private string _localHostName;

        internal ProxyFactory _proxyFactory
        {
            get;
            set;
        }

        public RosNode(Uri masterUri, string localHostName)
        {
            _masterClient = new MasterClient(masterUri);

            _proxyFactory = new ProxyFactory(_masterClient);

            _localHostName = localHostName;

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

            var publisher = new Publisher<TDataType>();

            var _slaveServer = new SlaveServer();
            _slaveServer.AcceptAsync().Subscribe(x => publisher.AddTopic(new RosTopic<TDataType>(x)));

            var channel = new HttpServerChannel("slave", 0, new XmlRpcServerFormatterSinkProvider());
            ChannelServices.RegisterChannel(channel, false);
            RemotingServices.Marshal(_slaveServer, "slave");

            var tmp = new Uri(channel.GetChannelUri());
            var slaveUri = new Uri("http://" + _localHostName + ":" + tmp.Port + "/slave");

            var ret1 = _masterClient.RegisterPublisherAsync("/test", "/chatter", "std_msgs/String", slaveUri).First();

            

            return publisher;
        }

        public Func<TRequest, IObservable<TResponse>> CreateProxy<TService, TRequest, TResponse>(string serviceName)
            where TService : IService<TRequest, TResponse>, new()
            where TRequest : IMessage, new()
            where TResponse : IMessage, new()
        {
            return _proxyFactory.Create<TService, TRequest, TResponse>(serviceName);
        }

        private Dictionary<string, Func<Stream, Stream>> _services = new Dictionary<string, Func<Stream, Stream>>();

        public IDisposable RegisterService<TService, TRequest, TResponse>(string serviceName, Func<TRequest, TResponse> service) where TService : IService<TRequest, TResponse>, new() where TRequest : IMessage, new() where TResponse : IMessage, new()
        {

            var _slaveServer = new SlaveServer();
            _slaveServer.AcceptAsync().Subscribe(x => Console.WriteLine(x.SocketType));

            var channel = new HttpServerChannel("slave", 0, new XmlRpcServerFormatterSinkProvider());
            
            
            ChannelServices.RegisterChannel(channel, false);
            RemotingServices.Marshal(_slaveServer, "slave");

            var func = new Func<Stream, Stream>(stream =>
            {
                var req = new TRequest();
                req.Deserialize(stream);
                var res = service(req);
                var ms = new MemoryStream();
                res.Serialize(ms);
                return ms;
            });



            var ret1 = _masterClient
                .RegisterServiceAsync("/test", "chatter", 
                new Uri("rostcp://192.168.11.2:11112"),
                new Uri("http://192.168.11.2:11112"))
                .First();//TODO: エラーが起きたとき


            throw new NotImplementedException();
        }



    }
}
