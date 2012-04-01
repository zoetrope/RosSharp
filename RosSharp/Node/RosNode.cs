#region License Terms

// ================================================================================
// RosSharp
// 
// Software License Agreement (BSD License)
// 
// Copyright (C) 2012 zoetrope
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// ================================================================================

#endregion

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
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();
        private readonly MasterClient _masterClient;
        private readonly ServiceProxyFactory _serviceProxyFactory;
        private readonly SlaveServer _slaveServer;
        private readonly TcpRosListener _tcpRosListener;
        private readonly TopicContainer _topicContainer;

        private ParameterServerClient _parameterServerClient;

        private Dictionary<string, IParameter> _parameters = new Dictionary<string, IParameter>();
        private Dictionary<string, IService> _services = new Dictionary<string, IService>();

        public RosNode(string nodeId)
        {
            _logger.InfoFormat("Create Node: {0}", nodeId);

            NodeId = nodeId;

            _masterClient = new MasterClient(ROS.MasterUri);
            _parameterServerClient = new ParameterServerClient(ROS.MasterUri);

            _serviceProxyFactory = new ServiceProxyFactory(NodeId);

            _topicContainer = new TopicContainer();
            _tcpRosListener = new TcpRosListener(0);
            _slaveServer = new SlaveServer(0, _topicContainer, _tcpRosListener);

            _slaveServer.ParameterUpdated += SlaveServerOnParameterUpdated;
        }

        public string NodeId { get; set; }

        #region INode Members

        public Parameter<T> GetParameter<T>(string paramName)
        {
            if (_parameters.ContainsKey(paramName))
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

            _tcpRosListener.AcceptAsync()
                .Subscribe(socket => publisher.AddTopic(socket));

            return _masterClient
                .RegisterPublisherAsync(NodeId, topicName, publisher.MessageType, _slaveServer.SlaveUri)
                .ContinueWith(task => publisher.UpdateSubscribers(task.Result))
                .ContinueWith(_ => publisher); //TODO: 例外起きたときは？
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

        #endregion

        private void SlaveServerOnParameterUpdated(string key, object value)
        {
            if (!_parameters.ContainsKey(key))
            {
                return;
            }

            var param = _parameters[key];
            param.Update(value);
        }
    }
}