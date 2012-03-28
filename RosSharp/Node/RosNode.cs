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
using System.Threading.Tasks;
using RosSharp.Transport;

namespace RosSharp.Node
{
    public class RosNode : INode
    {
        private readonly MasterClient _masterClient;
        private readonly SlaveServer _slaveServer;
        private readonly ServiceProxyFactory _serviceProxyFactory;
        private readonly RosTcpListener _rosTcpListener;
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
            _rosTcpListener = new RosTcpListener(0);
            _slaveServer = new SlaveServer(0, _topicContainer, _rosTcpListener);

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

        public Task<Subscriber<TDataType>> CreateSubscriber<TDataType>(string topicName) 
            where TDataType : IMessage, new()
        {
            _logger.InfoFormat("Create Subscriber: {0}", topicName);
            var subscriber = new Subscriber<TDataType>(topicName, NodeId);
            _topicContainer.AddSubscriber(subscriber);

            var dummy = new TDataType();

            return _masterClient
                .RegisterSubscriberAsync(NodeId, topicName, dummy.MessageType, _slaveServer.SlaveUri)
                .ContinueWith(task => ((ISubscriber) subscriber).UpdatePublishers(task.Result)) //TODO: 例外チェックは必要？
                .ContinueWith(_ => subscriber);
        }

        public Task RemoveSubscriber(string topicName)
        {
            return _masterClient
                .UnregisterSubscriberAsync(NodeId, topicName, _slaveServer.SlaveUri)
                .ContinueWith(task => _topicContainer.RemoveSubscriber(topicName));
        }


        public Task<Publisher<TDataType>> CreatePublisher<TDataType>(string topicName) 
            where TDataType : IMessage, new()
        {
            _logger.InfoFormat("Create Publisher: {0}", topicName);

            var publisher = new Publisher<TDataType>(topicName, NodeId);
            _topicContainer.AddPublisher(publisher);

            _rosTcpListener.AcceptAsync()
                .Subscribe(socket => publisher.AddTopic(socket));

            return _masterClient
                .RegisterPublisherAsync(NodeId, topicName, publisher.Type, _slaveServer.SlaveUri)
                .ContinueWith(task => publisher.UpdateSubscriber(task.Result))
                .ContinueWith(_ => publisher);
        }

        public Task RemovePublisher(string topicName)
        {
            return _masterClient
                .UnregisterPublisherAsync(NodeId, topicName, _slaveServer.SlaveUri)
                .ContinueWith(_ => _topicContainer.RemovePublisher(topicName));
        }

        public Task<TService> CreateProxy<TService>(string serviceName)
            where TService : IService, new()
        {
            _logger.InfoFormat("Create ServiceProxy: {0}", serviceName);

            return _masterClient.LookupServiceAsync(NodeId, serviceName)
                .ContinueWith(task => _serviceProxyFactory.Create<TService>(serviceName, task.Result));

        }

        private Dictionary<string, IService> _services = new Dictionary<string, IService>();

        public Task RegisterService<TService>(string serviceName, TService service) 
            where TService : IService, new()
        {
            _logger.InfoFormat("Create ServiceServer: {0}", serviceName);

            var serviceServer = new ServiceServer<TService>(NodeId);
            serviceServer.RegisterService(serviceName, service);
            _services.Add(serviceName, service);

            var serviceUri = new Uri("rosrpc://" + ROS.HostName + ":" + serviceServer.EndPoint.Port);

            return _masterClient
                .RegisterServiceAsync(NodeId, serviceName, serviceUri, _slaveServer.SlaveUri);
        }

        public Task RemoveService(string serviceName)
        {
            return _masterClient
                .UnregisterServiceAsync(NodeId, serviceName, _slaveServer.SlaveUri)
                .ContinueWith(_ => _services.Remove(serviceName));
        }
    }
}
