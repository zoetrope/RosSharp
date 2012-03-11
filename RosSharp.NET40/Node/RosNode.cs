using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Threading;
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
        private readonly MasterClient _masterClient;
        private readonly SlaveServer _slaveServer;
        private readonly ProxyFactory _proxyFactory;
        private readonly RosTopicServer _rosTopicServer;
        private readonly TopicContainer _topicContainer;

        public string NodeId { get; set; }

        public RosNode(string nodeId)
        {
            NodeId = nodeId;

            _masterClient = new MasterClient(ROS.MasterUri);
            
            _proxyFactory = new ProxyFactory(_masterClient);

            _topicContainer = new TopicContainer();
            _rosTopicServer = new RosTopicServer();
            _slaveServer = new SlaveServer(_topicContainer, _rosTopicServer);
        }

        public Subscriber<TDataType> CreateSubscriber<TDataType>(string topicName) 
            where TDataType : IMessage, new()
        {
            var dummy = new TDataType();

            var uri = _masterClient
                .RegisterSubscriberAsync(NodeId, topicName, dummy.MessageType, _slaveServer.SlaveUri)
                .First();//TODO: エラーが起きたとき

            var slave = new SlaveClient(uri.First());

            var topicParam = slave.RequestTopicAsync(NodeId, topicName, new object[1] { new string[1] { "TCPROS" } }).First();

            var subscriber = new Subscriber<TDataType>(topicName, topicParam);

            _topicContainer.AddSubscriber(subscriber);

            return subscriber;
        }
        
        public Publisher<TDataType> CreatePublisher<TDataType>(string topicName) 
            where TDataType : IMessage, new()
        {

            var publisher = new Publisher<TDataType>(topicName);

            _rosTopicServer.AcceptAsync().Subscribe(socket => publisher.AddTopic(new RosTopic<TDataType>(socket, NodeId, topicName)));


            var ret1 = _masterClient.RegisterPublisherAsync(NodeId, topicName, publisher.Type, _slaveServer.SlaveUri).First();

            _topicContainer.AddPublisher(publisher);

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

        public IDisposable RegisterService<TService, TRequest, TResponse>(string serviceName, Func<TRequest, TResponse> service) 
            where TService : IService<TRequest, TResponse>, new() 
            where TRequest : IMessage, new() 
            where TResponse : IMessage, new()
        {
            var serviceServer = new ServiceServer<TService, TRequest, TResponse>(NodeId);
            serviceServer.RegisterService(serviceName, service);

            var ret1 = _masterClient
                .RegisterServiceAsync(NodeId, serviceName, 
                    new Uri("rosrpc://"+ ROS.LocalHostName +":" + serviceServer.Port),
                    _slaveServer.SlaveUri)
                .First(); //TODO: Firstはだめ。


            return Disposable.Empty;//TODO: サービス登録を解除するためのDisposableを返す。
        }



    }
}
