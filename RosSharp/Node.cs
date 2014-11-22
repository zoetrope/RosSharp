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
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Common.Logging;
using RosSharp.Master;
using RosSharp.Message;
using RosSharp.Parameter;
using RosSharp.Service;
using RosSharp.Slave;
using RosSharp.Topic;
using RosSharp.Transport;
using RosSharp.Utility;
using RosSharp.roscpp;
using RosSharp.rosgraph_msgs;

namespace RosSharp
{
    /// <summary>
    ///   ROS Node
    /// </summary>
    public class Node : IAsyncDisposable
    {
        private readonly ILog _logger;
        private readonly MasterClient _masterClient;
        private readonly ParameterServerClient _parameterServerClient;
        private readonly Dictionary<string, IParameter> _parameters = new Dictionary<string, IParameter>();
        private readonly Dictionary<string, IServiceProxy> _serviceProxies = new Dictionary<string, IServiceProxy>();
        private readonly ServiceProxyFactory _serviceProxyFactory;
        private readonly Dictionary<string, IService> _serviceServers = new Dictionary<string, IService>();

        private readonly SlaveServer _slaveServer;
        private readonly TopicContainer _topicContainer;
        private bool _disposed;
        private Dictionary<string, IDisposable> _publisherDisposables = new Dictionary<string, IDisposable>();

        public Node(string nodeId)
        {
            _disposed = false;
            
            NodeId = nodeId;

            _logger = RosOutLogManager.GetCurrentNodeLogger(NodeId);

            if (_logger.IsDebugEnabled)
            {
                LogLevel = Log.DEBUG;
            }
            else if (_logger.IsInfoEnabled)
            {
                LogLevel = Log.INFO;
            }
            else if (_logger.IsWarnEnabled)
            {
                LogLevel = Log.WARN;
            }
            else if (_logger.IsErrorEnabled)
            {
                LogLevel = Log.ERROR;
            }
            else if (_logger.IsFatalEnabled)
            {
                LogLevel = Log.FATAL;
            }

            _masterClient = new MasterClient(Ros.MasterUri);
            _parameterServerClient = new ParameterServerClient(Ros.MasterUri);

            _serviceProxyFactory = new ServiceProxyFactory(NodeId);

            _topicContainer = new TopicContainer();
            _slaveServer = new SlaveServer(NodeId, 0, _topicContainer);

            _slaveServer.ParameterUpdated += SlaveServerOnParameterUpdated;

            _logger.InfoFormat("Create Node: {0}", nodeId);
        }

        internal byte LogLevel { get; private set; }

        internal Publisher<Log> LogPubliser { get; private set; }

        /// <summary>
        ///   Node ID
        /// </summary>
        public string NodeId { get; private set; }

        #region IAsyncDisposable Members

        public void Dispose()
        {
            DisposeAsync().Wait();
        }

        public Task DisposeAsync()
        {
            if (_disposed) throw new ObjectDisposedException("Node");
            _disposed = true;

            var tasks = new List<Task>();

            tasks.AddRange(_topicContainer.GetPublishers().Select(pub => pub.DisposeAsync()));
            tasks.AddRange(_topicContainer.GetSubscribers().Select(sub => sub.DisposeAsync()));

            tasks.AddRange(_serviceProxies.Values.Select(proxy => proxy.DisposeAsync()));
            tasks.AddRange(_serviceServers.Values.Select(service => service.DisposeAsync()));

            tasks.AddRange(_parameters.Values.Select(param => param.DisposeAsync()));

            return Task.Factory.StartNew(() =>
            {
                try
                {
                    Task.WaitAll(tasks.ToArray());
                }
                catch (Exception ex)
                {
                    _logger.Error("Node Dispose Error", ex);
                }
                
                var handler = Disposing;
                Disposing = null;

                if (handler != null)
                {
                    handler(NodeId);
                }

                _slaveServer.Dispose();
            });
        }

        public event Func<string, Task> Disposing = _ => Task.Factory.StartNew(() => { });

        #endregion

        /// <summary>
        ///   Create a Primitive Parameter
        /// </summary>
        /// <typeparam name="T"> Parameter Type (primitive type only)</typeparam>
        /// <param name="paramName"> Parameter Name </param>
        /// <returns> Parameter </returns>
        public Task<PrimitiveParameter<T>> PrimitiveParameterAsync<T>(string paramName)
        {
            return CreateParameterAsync<PrimitiveParameter<T>>(paramName);
        }

        /// <summary>
        ///   Create a List Parameter
        /// </summary>
        /// <typeparam name="T"> Parameter Type (primitive type only)</typeparam>
        /// <param name="paramName"> Parameter Name </param>
        /// <returns> Parameter </returns>
        public Task<ListParameter<T>> ListParameterAsync<T>(string paramName)
        {
            return CreateParameterAsync<ListParameter<T>>(paramName);
        }

        /// <summary>
        ///   Create a Dynamic Parameter
        /// </summary>
        /// <param name="paramName"> Parameter Name </param>
        /// <returns> Parameter </returns>
        public async Task<DynamicParameter> DynamicParameterAsync(string paramName)
        {
            return await CreateParameterAsync<DynamicParameter>(paramName);
        }

        private async Task<T> CreateParameterAsync<T>(string paramName)
            where T : IParameter, new()
        {
            if (_disposed) throw new ObjectDisposedException("Node");

            if (_parameters.ContainsKey(paramName))
            {
                throw new InvalidOperationException(paramName + " is already created.");
            }

            var param = new T();
            param.Disposing += DisposeParameter;

            _parameters.Add(paramName, param);

            try
            {
                await param.InitializeAsync(NodeId, paramName, _slaveServer.SlaveUri, _parameterServerClient);
            }
            catch (Exception ex) //TODO:
            {
                _logger.Error("Initialize Parameter: Failure", ex);
                throw;
            }

            return param;
        }


        /// <summary>
        ///   Create a ROS Topic Subscriber
        /// </summary>
        /// <typeparam name="TMessage"> Topic Message Type </typeparam>
        /// <param name="topicName"> Topic Name </param>
        /// <param name="nodelay"> false: Socket uses the Nagle algorithm </param>
        /// <returns> Subscriber </returns>
        public async Task<Subscriber<TMessage>> SubscriberAsync<TMessage>(string topicName, bool nodelay = true)
            where TMessage : IMessage, new()
        {
            if (_disposed) throw new ObjectDisposedException("Node");

            if (_topicContainer.HasSubscriber(topicName))
            {
                throw new InvalidOperationException(topicName + " is already created.");
            }

            _logger.InfoFormat("Create Subscriber: {0}", topicName);

            var subscriber = new Subscriber<TMessage>(topicName, NodeId, nodelay);
            _topicContainer.AddSubscriber(subscriber);
            subscriber.Disposing += DisposeSubscriberAsync;

            _logger.Debug("RegisterSubscriber");
            try
            {
                var result = await _masterClient
                    .RegisterSubscriberAsync(NodeId, topicName, subscriber.MessageType, _slaveServer.SlaveUri);
                _logger.Debug("Registered Subscriber");
                await ((ISubscriber)subscriber).UpdatePublishers(result);
            }
            catch (Exception ex) //TODO:
            {
                _logger.Error("RegisterSubscriber: Failure", ex);
                throw;
            }
            return subscriber;
        }

        /// <summary>
        ///   Create a ROS Topic Publisher
        /// </summary>
        /// <typeparam name="TMessage"> Topic Message Type </typeparam>
        /// <param name="topicName"> Topic Name </param>
        /// <param name="latching"> true: send the latest published message when subscribed topic </param>
        /// <returns> Publisher </returns>
        public async Task<Publisher<TMessage>> PublisherAsync<TMessage>(string topicName, bool latching = false)
            where TMessage : IMessage, new()
        {
            if (_disposed) throw new ObjectDisposedException("Node");

            if (_topicContainer.HasPublisher(topicName))
            {
                throw new InvalidOperationException(topicName + " is already created.");
            }

            _logger.InfoFormat("Create Publisher: {0}", topicName);

            var publisher = new Publisher<TMessage>(topicName, NodeId, latching);
            _topicContainer.AddPublisher(publisher);
            publisher.Disposing += DisposePublisherAsync;

            var tcpRosListener = new TcpRosListener(0);
            _slaveServer.AddListener(topicName, tcpRosListener);

            var acceptDisposable = tcpRosListener.AcceptAsync()
                .Do(_ => _logger.Debug("Accepted for Publisher"))
                .Subscribe(socket => publisher.AddTopic(socket),
                           ex => _logger.Error("Accept Error", ex));

            _publisherDisposables.Add(topicName, acceptDisposable);

            _logger.Debug("RegisterPublisher");
            try
            {
                var result = await _masterClient
                    .RegisterPublisherAsync(NodeId, topicName, publisher.MessageType, _slaveServer.SlaveUri);
                _logger.Debug("Registered Publisher");
                publisher.UpdateSubscribers(result);
            }
            catch (Exception ex) //TODO:
            {
                _logger.Error("RegisterPublisher: Failure", ex);
                throw;
            }
            return publisher;
        }

        /// <summary>
        ///   Create a Proxy Object for ROS Service
        /// </summary>
        /// <typeparam name="TService"> Service Type </typeparam>
        /// <param name="serviceName"> Service Name </param>
        /// <returns> Proxy Object </returns>
        public async Task<TService> ServiceProxyAsync<TService>(string serviceName)
            where TService : IService, new()
        {
            if (_disposed) throw new ObjectDisposedException("Node");
            if (_serviceProxies.ContainsKey(serviceName))
            {
                throw new InvalidOperationException(serviceName + " is already created.");
            }

            _logger.InfoFormat("Create ServiceProxy: {0}", serviceName);

            try
            {
                var urls = await _masterClient.LookupServiceAsync(NodeId, serviceName);
                _logger.Debug("Registered Subscriber");
                var proxy = await _serviceProxyFactory.CreateAsync<TService>(serviceName, urls);
                proxy.Disposing += DisposeProxyAsync;
                _serviceProxies.Add(serviceName, proxy);
                return proxy.Service;
            }
            catch (Exception ex)//TODO:
            {
                _logger.Error("RegisterSubscriber: Failure", ex);
                throw;
            }
            
        }

        public Task WaitForService(string serviceName)
        {
            return Observable.Defer(() => _masterClient.LookupServiceAsync(NodeId, serviceName).ToObservable())
                .RetryWithDelay(TimeSpan.FromSeconds(5))
                .Take(1)
                .ToTask();
        }

        /// <summary>
        ///   Register a ROS Service
        /// </summary>
        /// <typeparam name="TService"> Service Type </typeparam>
        /// <param name="serviceName"> Service Name </param>
        /// <param name="service"> Service Instance </param>
        /// <returns> object that dispose a service </returns>
        public async Task<IServiceServer> AdvertiseServiceAsync<TService>(string serviceName, TService service)
            where TService : IService, new()
        {
            if (_disposed) throw new ObjectDisposedException("Node");
            if (_serviceServers.ContainsKey(serviceName))
            {
                throw new InvalidOperationException(serviceName + " is already registered.");
            }

            _logger.InfoFormat("Create ServiceServer: {0}", serviceName);

            var serviceServer = new ServiceServer<TService>(NodeId);
            serviceServer.StartService(serviceName, service);
            serviceServer.Disposing += DisposeServiceAsync;

            _serviceServers.Add(serviceName, service);

            var serviceUri = new Uri("rosrpc://" + Ros.HostName + ":" + serviceServer.EndPoint.Port);

            await _masterClient.RegisterServiceAsync(NodeId, serviceName, serviceUri, _slaveServer.SlaveUri);
            return serviceServer;
        }

        internal Task InitializeAsync(bool enableLogger)
        {
            if (enableLogger)
            {
                var t1 = PublisherAsync<Log>("/rosout").ContinueWith(t => LogPubliser = t.Result);

                var t2 = AdvertiseServiceAsync(NodeId + "/get_loggers", new GetLoggers(GetLoggers));
                var t3 = AdvertiseServiceAsync(NodeId + "/set_logger_level", new SetLoggerLevel(SetLoggerLevel));

                return Task.Factory.StartNew(() =>
                {
                    Task.WaitAll(new Task[] {t1, t2, t3});
                    _logger.Info(m => m("Created Node = {0}", NodeId));
                });
            }
            else
            {
                return Task.Factory.StartNew(() => { _logger.Info(m => m("Created Node = {0}", NodeId)); });
            }
        }


        internal SetLoggerLevel.Response SetLoggerLevel(SetLoggerLevel.Request request)
        {
            if (request.logger == "RosSharp")
            {
                byte level;
                if (LogLevelExtensions.TryParse(request.level, out level))
                {
                    LogLevel = level;
                }
            }

            return new SetLoggerLevel.Response();
        }

        internal GetLoggers.Response GetLoggers(GetLoggers.Request request)
        {
            return new GetLoggers.Response()
            {
                loggers = new List<Logger>()
                {
                    new Logger() {name = "RosSharp", level = LogLevel.ToLogLevelString()}
                }
            };
        }

        private async Task DisposeSubscriberAsync(string topicName)
        {
            _logger.Debug(m => m("Disposing Subscriber[{0}]", topicName));

            try
            {
                var result = await _masterClient.UnregisterSubscriberAsync(NodeId, topicName, _slaveServer.SlaveUri);
            }
            catch (Exception ex)
            {
                _logger.Error("UnregisterSubscriber: Failure", ex);
            }
            _topicContainer.RemoveSubscriber(topicName);
            _logger.Debug(m => m("UnregisterSubscriber: [{0}]", topicName));
        }

        private async Task DisposePublisherAsync(string topicName)
        {
            _logger.Debug(m => m("Disposing Publisher[{0}]", topicName));

            try
            {
                var result = await _masterClient.UnregisterPublisherAsync(NodeId, topicName, _slaveServer.SlaveUri);
            }
            catch (Exception ex)
            {
                _logger.Error("UnregisterPublisher: Failure", ex);
            }
            _topicContainer.RemovePublisher(topicName);
            _slaveServer.RemoveListener(topicName);

            _publisherDisposables[topicName].Dispose();
            _publisherDisposables.Remove(topicName);
            _logger.Debug(m => m("UnregisterPublisher: [{0}]", topicName));
        }

        private Task DisposeServiceAsync(string serviceName)
        {
            return _masterClient
                .UnregisterServiceAsync(NodeId, serviceName, _slaveServer.SlaveUri)
                .ContinueWith(_ => _serviceServers.Remove(serviceName));
        }

        private Task DisposeProxyAsync(string serviceName)
        {
            return Task.Factory.StartNew(() => _serviceProxies.Remove(serviceName));
        }

        private Task DisposeParameter(string paramName)
        {
            return Task.Factory.StartNew(() => { _parameters.Remove(paramName); });
        }

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