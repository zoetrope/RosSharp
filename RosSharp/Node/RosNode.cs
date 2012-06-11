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
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
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

namespace RosSharp.Node
{
    /// <summary>
    ///   ROS Node
    /// </summary>
    public class RosNode : IAsyncDisposable
    {
        private readonly ILog _logger = LogManager.GetCurrentClassLogger();
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

        public RosNode(string nodeId)
        {
            LogLevel = Log.INFO;

            _logger.InfoFormat("Create Node: {0}", nodeId);

            NodeId = nodeId;

            _masterClient = new MasterClient(Ros.MasterUri);
            _parameterServerClient = new ParameterServerClient(Ros.MasterUri);

            _serviceProxyFactory = new ServiceProxyFactory(NodeId);

            _topicContainer = new TopicContainer();
            _slaveServer = new SlaveServer(NodeId, 0, _topicContainer);

            _slaveServer.ParameterUpdated += SlaveServerOnParameterUpdated;
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
            if (_disposed) throw new ObjectDisposedException("RosNode");
            _disposed = true;

            var tasks = new List<Task>();

            tasks.AddRange(_topicContainer.GetPublishers().Select(pub => pub.DisposeAsync()));
            tasks.AddRange(_topicContainer.GetSubscribers().Select(sub => sub.DisposeAsync()));

            tasks.AddRange(_serviceProxies.Values.Select(proxy => proxy.DisposeAsync()));
            tasks.AddRange(_serviceServers.Values.Select(service => service.DisposeAsync()));

            tasks.AddRange(_parameters.Values.Select(param => param.DisposeAsync()));

            return Task.Factory.StartNew(() =>
            {
                Task.WaitAll(tasks.ToArray()); //TODO: 例外が起きたら他の処理が行われない・・・
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
        ///   Create a ROS Parameter
        /// </summary>
        /// <typeparam name="T"> Parameter Type </typeparam>
        /// <param name="paramName"> Parameter Name </param>
        /// <returns> Parameter </returns>
        public Task<PrimitiveParameter<T>> CreatePrimitiveParameterAsync<T>(string paramName)
        {
            return CreateParameterAsync<PrimitiveParameter<T>>(paramName);
        }

        public Task<ListParameter<T>> CreateListParameterAsync<T>(string paramName)
        {
            return CreateParameterAsync<ListParameter<T>>(paramName);
        }

        public Task<DynamicParameter> CreateDynamicParameterAsync(string paramName)
        {
            return CreateParameterAsync<DynamicParameter>(paramName);
        }

        private Task<T> CreateParameterAsync<T>(string paramName)
            where T : IParameter, new()
        {
            if (_disposed) throw new ObjectDisposedException("RosNode");

            if (_parameters.ContainsKey(paramName))
            {
                throw new InvalidOperationException(paramName + " is already created.");
            }

            var param = new T();
            param.Disposing += DisposeParameter;

            _parameters.Add(paramName, param);

            var tcs = new TaskCompletionSource<T>();

            param.InitializeAsync(NodeId, paramName, _slaveServer.SlaveUri, _parameterServerClient).ContinueWith(task =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    tcs.SetResult(param);
                }
                else if (task.Status == TaskStatus.Faulted)
                {
                    _logger.Error("Initialize Parameter: Failure", task.Exception.InnerException);
                    tcs.SetException(task.Exception.InnerException);
                }
            });

            return tcs.Task;
        }


        /// <summary>
        ///   Create a ROS Topic Subscriber
        /// </summary>
        /// <typeparam name="TMessage"> Topic Message Type </typeparam>
        /// <param name="topicName"> Topic Name </param>
        /// <param name="nodelay"> false: Socket uses the Nagle algorithm </param>
        /// <returns> Subscriber </returns>
        public Task<Subscriber<TMessage>> CreateSubscriberAsync<TMessage>(string topicName, bool nodelay = true)
            where TMessage : IMessage, new()
        {
            if (_disposed) throw new ObjectDisposedException("RosNode");

            if (_topicContainer.HasSubscriber(topicName))
            {
                throw new InvalidOperationException(topicName + " is already created.");
            }

            _logger.InfoFormat("Create Subscriber: {0}", topicName);

            var subscriber = new Subscriber<TMessage>(topicName, NodeId, nodelay);
            _topicContainer.AddSubscriber(subscriber);
            subscriber.Disposing += DisposeSubscriberAsync;

            var tcs = new TaskCompletionSource<Subscriber<TMessage>>();

            _logger.Debug("RegisterSubscriber");
            _masterClient
                .RegisterSubscriberAsync(NodeId, topicName, subscriber.MessageType, _slaveServer.SlaveUri)
                .ContinueWith(task =>
                {
                    _logger.Debug("Registered Subscriber");

                    if (task.Status == TaskStatus.RanToCompletion)
                    {
                        ((ISubscriber) subscriber).UpdatePublishers(task.Result);
                        tcs.SetResult(subscriber);
                    }
                    else if (task.Status == TaskStatus.Faulted)
                    {
                        tcs.SetException(task.Exception.InnerException);
                        _logger.Error("RegisterSubscriber: Failure", task.Exception.InnerException);
                    }
                });

            return tcs.Task;
        }

        /// <summary>
        ///   Create a ROS Topic Publisher
        /// </summary>
        /// <typeparam name="TMessage"> Topic Message Type </typeparam>
        /// <param name="topicName"> Topic Name </param>
        /// <param name="latching"> true: send the latest published message when subscribed topic </param>
        /// <returns> Publisher </returns>
        public Task<Publisher<TMessage>> CreatePublisherAsync<TMessage>(string topicName, bool latching = false)
            where TMessage : IMessage, new()
        {
            if (_disposed) throw new ObjectDisposedException("RosNode");

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

            var tcs = new TaskCompletionSource<Publisher<TMessage>>();

            _logger.Debug("RegisterPublisher");
            _masterClient
                .RegisterPublisherAsync(NodeId, topicName, publisher.MessageType, _slaveServer.SlaveUri)
                .ContinueWith(task =>
                {
                    _logger.Debug("Registered Publisher");

                    if (task.Status == TaskStatus.RanToCompletion)
                    {
                        publisher.UpdateSubscribers(task.Result);
                        tcs.SetResult(publisher);
                    }
                    else if (task.Status == TaskStatus.Faulted)
                    {
                        _logger.Error("RegisterPublisher: Failure", task.Exception.InnerException);
                        tcs.SetException(task.Exception.InnerException);
                    }
                });

            return tcs.Task;
        }

        /// <summary>
        ///   Create a Proxy Object for ROS Service
        /// </summary>
        /// <typeparam name="TService"> Service Type </typeparam>
        /// <param name="serviceName"> Service Name </param>
        /// <returns> Proxy Object </returns>
        public Task<TService> CreateProxyAsync<TService>(string serviceName)
            where TService : IService, new()
        {
            if (_disposed) throw new ObjectDisposedException("RosNode");
            if (_serviceProxies.ContainsKey(serviceName))
            {
                throw new InvalidOperationException(serviceName + " is already created.");
            }

            _logger.InfoFormat("Create ServiceProxy: {0}", serviceName);

            var tcs = new TaskCompletionSource<TService>();

            _masterClient
                .LookupServiceAsync(NodeId, serviceName)
                .ContinueWith(lookupTask =>
                {
                    _logger.Debug("Registered Subscriber");

                    if (lookupTask.Status == TaskStatus.RanToCompletion)
                    {
                        _serviceProxyFactory.CreateAsync<TService>(serviceName, lookupTask.Result)
                            .ContinueWith(createTask =>
                            {
                                if (createTask.Status == TaskStatus.RanToCompletion)
                                {
                                    var proxy = createTask.Result;
                                    proxy.Disposing += DisposeProxyAsync;
                                    _serviceProxies.Add(serviceName, proxy);
                                    tcs.SetResult(proxy.Service);
                                }
                                else if (createTask.Status == TaskStatus.Faulted)
                                {
                                    tcs.SetException(createTask.Exception.InnerException);
                                }
                            });
                    }
                    else if (lookupTask.Status == TaskStatus.Faulted)
                    {
                        tcs.SetException(lookupTask.Exception.InnerException);
                        _logger.Error("RegisterSubscriber: Failure", lookupTask.Exception.InnerException);
                    }
                });

            return tcs.Task;
        }

        /// <summary>
        ///   Register a ROS Service
        /// </summary>
        /// <typeparam name="TService"> Service Type </typeparam>
        /// <param name="serviceName"> Service Name </param>
        /// <param name="service"> Service Instance </param>
        /// <returns> object that dispose a service </returns>
        public Task<IServiceServer> RegisterServiceAsync<TService>(string serviceName, TService service)
            where TService : IService, new()
        {
            if (_disposed) throw new ObjectDisposedException("RosNode");
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

            var tcs = new TaskCompletionSource<IServiceServer>();

            _masterClient
                .RegisterServiceAsync(NodeId, serviceName, serviceUri, _slaveServer.SlaveUri)
                .ContinueWith(registerTask =>
                {
                    if (registerTask.Status == TaskStatus.RanToCompletion)
                    {
                        tcs.SetResult(serviceServer);
                    }
                    else if (registerTask.Status == TaskStatus.Faulted)
                    {
                        tcs.SetException(registerTask.Exception.InnerException);
                    }
                });
            return tcs.Task;
        }

        internal Task InitializeAsync(bool enableLogger)
        {
            if (enableLogger)
            {
                var t1 = CreatePublisherAsync<Log>("/rosout").ContinueWith(t => LogPubliser = t.Result);

                var t2 = RegisterServiceAsync(NodeId + "/get_loggers", new GetLoggers(GetLoggers));
                var t3 = RegisterServiceAsync(NodeId + "/set_logger_level", new SetLoggerLevel(SetLoggerLevel));

                return Task.Factory.StartNew(() => Task.WaitAll(new Task[] {t1, t2, t3}));
            }
            else
            {
                return Task.Factory.StartNew(() => { });
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

        private Task DisposeSubscriberAsync(string topicName)
        {
            _logger.Debug(m => m("Disposing Subscriber[{0}]", topicName));

            return _masterClient
                .UnregisterSubscriberAsync(NodeId, topicName, _slaveServer.SlaveUri)
                .ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        _logger.Error("UnregisterSubscriber: Failure", task.Exception.InnerException);
                    }
                    _topicContainer.RemoveSubscriber(topicName);
                    _logger.Debug(m => m("UnregisterSubscriber: [{0}]", topicName));
                });
        }

        private Task DisposePublisherAsync(string topicName)
        {
            _logger.Debug(m => m("Disposing Publisher[{0}]", topicName));

            return _masterClient
                .UnregisterPublisherAsync(NodeId, topicName, _slaveServer.SlaveUri)
                .ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        _logger.Error("UnregisterPublisher: Failure", task.Exception.InnerException);
                    }
                    _topicContainer.RemovePublisher(topicName);
                    _slaveServer.RemoveListener(topicName);

                    _publisherDisposables[topicName].Dispose();
                    _publisherDisposables.Remove(topicName);
                    _logger.Debug(m => m("UnregisterPublisher: [{0}]", topicName));
                });
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