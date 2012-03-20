using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using RosSharp.Master;
using RosSharp.Message;
using RosSharp.Service;
using RosSharp.Slave;
using RosSharp.Topic;

namespace RosSharp.Node
{
    public class RosNode : INode
    {
        private readonly MasterClient _masterClient;
        private readonly SlaveServer _slaveServer;
        private readonly ServiceProxyFactory _serviceProxyFactory;
        private readonly RosTopicServer _rosTopicServer;
        private readonly TopicContainer _topicContainer;

        public string NodeId { get; set; }

        public RosNode(string nodeId)
        {
            NodeId = nodeId;

            _masterClient = new MasterClient(ROS.MasterUri);
            
            _serviceProxyFactory = new ServiceProxyFactory(NodeId);

            _topicContainer = new TopicContainer();
            _rosTopicServer = new RosTopicServer();
            _slaveServer = new SlaveServer(0, _topicContainer, _rosTopicServer);
        }

        public Subscriber<TDataType> CreateSubscriber<TDataType>(string topicName) 
            where TDataType : IMessage, new()
        {
            var dummy = new TDataType();

            var uris = _masterClient
                .RegisterSubscriberAsync(NodeId, topicName, dummy.MessageType, _slaveServer.SlaveUri)
                .First();//TODO: エラーが起きたとき

            //TODO: 複数のSlave対応？
            var slave = new SlaveClient(uris.First()); //TODO: Subscriberに持たせる

            var topicParam = slave.RequestTopicAsync(
                NodeId, topicName, new object[1] {new string[1] {"TCPROS"}}).First();

            var subscriber = new Subscriber<TDataType>(topicName, NodeId);
            subscriber.Connect(topicParam);

            _topicContainer.AddSubscriber(subscriber);

            return subscriber;
        }
        
        public Publisher<TDataType> CreatePublisher<TDataType>(string topicName) 
            where TDataType : IMessage, new()
        {

            var publisher = new Publisher<TDataType>(topicName,NodeId);

            _rosTopicServer.AcceptAsync().Subscribe(socket => publisher.AddTopic(new RosTopicClient<TDataType>(socket, NodeId, topicName)));


            var uris = _masterClient.RegisterPublisherAsync(NodeId, topicName, publisher.Type, _slaveServer.SlaveUri).First();

            var slave = new SlaveClient(uris.First());//TODO: publisherに持たせる

            _topicContainer.AddPublisher(publisher);

            return publisher;
        }

        public Func<TRequest, IObservable<TResponse>> CreateProxy<TService, TRequest, TResponse>(string serviceName)
            where TService : IService<TRequest, TResponse>, new()
            where TRequest : IMessage, new()
            where TResponse : IMessage, new()
        {
            var uri = _masterClient.LookupServiceAsync(NodeId, serviceName).First();

            return _serviceProxyFactory.Create<TService, TRequest, TResponse>(serviceName, uri);
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
                    new Uri("rosrpc://" + ROS.LocalHostName + ":" + serviceServer.EndPoint.Port),
                    _slaveServer.SlaveUri)
                .First(); //TODO: Firstはだめ。


            return Disposable.Empty;//TODO: サービス登録を解除するためのDisposableを返す。
        }



    }
}
