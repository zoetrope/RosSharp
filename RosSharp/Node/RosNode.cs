using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Common.Logging;
using RosSharp.Master;
using RosSharp.Message;
using RosSharp.Parameter;
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

        private ParameterServerClient _parameterServerClient;

        private readonly ILog _logger = LogManager.GetCurrentClassLogger();

        public string NodeId { get; set; }

        public RosNode(string nodeId)
        {
            _logger.InfoFormat("Create Node: {0}", nodeId);

            NodeId = nodeId;

            _masterClient = new MasterClient(ROS.MasterUri);
            _parameterServerClient = new ParameterServerClient(ROS.MasterUri);

            _serviceProxyFactory = new ServiceProxyFactory(NodeId);

            _topicContainer = new TopicContainer();
            _rosTopicServer = new RosTopicServer();
            _slaveServer = new SlaveServer(0, _topicContainer, _rosTopicServer);

            _slaveServer.ParameterUpdated += SlaveServerOnParameterUpdated;
        }

        private void SlaveServerOnParameterUpdated(string key, object value)
        {
            if(!_parameters.ContainsKey(key))
            {
                return;
            }

            var param = _parameters[key];
            param.Update(value);
        }

        private Dictionary<string, IParameter> _parameters = new Dictionary<string, IParameter>();

        public Parameter<T> GetParameter<T>(string paramName)
        {
            if(_parameters.ContainsKey(paramName))
            {
                //return _pa
            }
            return new Parameter<T>(NodeId, paramName, _slaveServer.SlaveUri, _parameterServerClient);
        }

        public void Dispose()
        {
            // すべてを終了させる。

            //終了待ち
        }

        public Subscriber<TDataType> CreateSubscriber<TDataType>(string topicName) 
            where TDataType : IMessage, new()
        {
            _logger.InfoFormat("Create Subscriber: {0}", topicName);
            var subscriber = new Subscriber<TDataType>(topicName, NodeId);
            _topicContainer.AddSubscriber(subscriber);

            var dummy = new TDataType();
            _masterClient
                .RegisterSubscriberAsync(NodeId, topicName, dummy.MessageType, _slaveServer.SlaveUri)
                .Subscribe(((ISubscriber) subscriber).UpdatePublishers);

            return subscriber;
        }

        public void RemoveSubscriber(string topicName)
        {
            _masterClient
                .UnregisterSubscriberAsync(NodeId, topicName, _slaveServer.SlaveUri)
                .Subscribe(_ => _topicContainer.RemoveSubscriber(topicName));
        }


        public Publisher<TDataType> CreatePublisher<TDataType>(string topicName) 
            where TDataType : IMessage, new()
        {
            _logger.InfoFormat("Create Publisher: {0}", topicName);

            var publisher = new Publisher<TDataType>(topicName, NodeId);
            _topicContainer.AddPublisher(publisher);

            _rosTopicServer.AcceptAsync()
                .Subscribe(socket => publisher.AddTopic(new RosTopicClient<TDataType>(socket, NodeId, topicName)));

            _masterClient
                .RegisterPublisherAsync(NodeId, topicName, publisher.Type, _slaveServer.SlaveUri)
                .Subscribe(publisher.UpdateSubscriber);

            return publisher;
        }

        public void RemovePublisher(string topicName)
        {
            _masterClient
                .UnregisterPublisherAsync(NodeId, topicName, _slaveServer.SlaveUri)
                .Subscribe(_ => _topicContainer.RemovePublisher(topicName));
        }

        public Func<TRequest, IObservable<TResponse>> CreateProxy<TService, TRequest, TResponse>(string serviceName)
            where TService : IService<TRequest, TResponse>, new()
            where TRequest : IMessage, new()
            where TResponse : IMessage, new()
        {
            _logger.InfoFormat("Create ServiceProxy: {0}", serviceName);

            var uri = _masterClient.LookupServiceAsync(NodeId, serviceName).First();

            return _serviceProxyFactory.Create<TService, TRequest, TResponse>(serviceName, uri);
        }

        public void RemoveServiceProxy(string serviceName)
        {
            
        }

        private Dictionary<string, Func<Stream, Stream>> _services = new Dictionary<string, Func<Stream, Stream>>();

        public IDisposable RegisterService<TService, TRequest, TResponse>(string serviceName, Func<TRequest, TResponse> service) 
            where TService : IService<TRequest, TResponse>, new() 
            where TRequest : IMessage, new() 
            where TResponse : IMessage, new()
        {
            _logger.InfoFormat("Create ServiceServer: {0}", serviceName);

            var serviceServer = new ServiceServer<TService, TRequest, TResponse>(NodeId);
            serviceServer.RegisterService(serviceName, service);

            var ret1 = _masterClient
                .RegisterServiceAsync(NodeId, serviceName,
                    new Uri("rosrpc://" + ROS.HostName + ":" + serviceServer.EndPoint.Port),
                    _slaveServer.SlaveUri)
                .First(); //TODO: Firstはだめ。


            return Disposable.Empty;//TODO: サービス登録を解除するためのDisposableを返す。
        }

        public void RemoveService(string serviceName)
        {
            _masterClient
                .UnregisterServiceAsync(NodeId, serviceName, _slaveServer.SlaveUri)
                .Subscribe(_ => _services.Remove(serviceName));
        }
    }
}
