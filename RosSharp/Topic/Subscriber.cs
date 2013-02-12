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
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Common.Logging;
using RosSharp.Message;
using RosSharp.Slave;
using RosSharp.Utility;

namespace RosSharp.Topic
{
    /// <summary>
    ///   Subscribes message on a ROS Topic
    /// </summary>
    /// <typeparam name="TMessage"> Message Type </typeparam>
    public sealed class Subscriber<TMessage> : ISubscriber, IObservable<TMessage>
        where TMessage : IMessage, new()
    {
        private readonly Subject<TMessage> _aggregateSubject = new Subject<TMessage>();
        private readonly BehaviorSubject<int> _connectionCounterSubject = new BehaviorSubject<int>(0);
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private readonly ILog _logger;
        private readonly bool _nodelay;
        private readonly List<RosTopicServer<TMessage>> _rosTopicServers = new List<RosTopicServer<TMessage>>();

        internal Subscriber(string topicName, string nodeId, bool nodelay = true)
        {
            NodeId = nodeId;
            _logger = RosOutLogManager.GetCurrentNodeLogger(NodeId);
            TopicName = topicName;
            var dummy = new TMessage();
            MessageType = dummy.MessageType;
            _nodelay = nodelay;
        }

        public string NodeId { get; private set; }

        #region IObservable<TMessage> Members

        public IDisposable Subscribe(IObserver<TMessage> observer)
        {
            return _aggregateSubject.Subscribe(observer);
        }

        #endregion

        #region ISubscriber Members

        public Task DisposeAsync()
        {
            lock (_rosTopicServers)
            {
                foreach (var server in _rosTopicServers)
                {
                    server.Dispose();
                }
                _rosTopicServers.Clear();
            }

            lock (_disposables)
            {
                _disposables.Dispose();
            }

            _aggregateSubject.Dispose();

            var handler = Disposing;
            Disposing = null;
            return handler(TopicName);
        }

        public void Dispose()
        {
            DisposeAsync().Wait();
        }

        async Task ISubscriber.UpdatePublishers(List<Uri> publishers)
        {
            _logger.Debug("UpdatePublishers");

            List<SlaveClient> slaves;
            lock (_rosTopicServers)
            {
                slaves = publishers
                    .Except(_rosTopicServers.Select(server => server.SlaveUri))
                    .Select(x => new SlaveClient(x))
                    .ToList();
            }

            foreach (var slaveClient in slaves)
            {
                var uri = slaveClient.SlaveUri;
                try
                {
                    var param = await slaveClient.RequestTopicAsync(NodeId, TopicName, new List<ProtocolInfo> { new ProtocolInfo(ProtocolType.TCPROS) });
                    await ConnectServer(param, uri);
                }
                catch (Exception ex)
                {
                    _logger.Error(m => m("RequestTopicAsync Failure :{0}", uri), ex);
                }
            }
        }

        public string TopicName { get; private set; }

        public string MessageType { get; private set; }
        public event Func<string, Task> Disposing = _ => Task.Factory.StartNew(() => { });

        #endregion

        private async Task ConnectServer(TopicParam param, Uri slaveUri)
        {
            var server = new RosTopicServer<TMessage>(NodeId, TopicName, slaveUri);
            lock (_rosTopicServers)
            {
                _rosTopicServers.Add(server);
            }

            try
            {
                var subs = await server.StartAsync(param, _nodelay);
                var lazyDisposable = new SingleAssignmentDisposable();

                var d = subs.Subscribe(
                    x => _aggregateSubject.OnNext(x),
                    ex =>
                    {
                        lock (_rosTopicServers)
                        {
                            var index = _rosTopicServers.FindIndex(x => x.SlaveUri == slaveUri);
                            if (index != -1)
                            {
                                _rosTopicServers.RemoveAt(index);
                                _connectionCounterSubject.OnNext(_rosTopicServers.Count);
                                lazyDisposable.Dispose();
                            }
                        }
                        //_aggregateSubject.OnError(ex);
                    }, () =>
                    {
                        lock (_rosTopicServers)
                        {
                            var index = _rosTopicServers.FindIndex(x => x.SlaveUri == slaveUri);
                            if (index != -1)
                            {
                                _rosTopicServers.RemoveAt(index);
                                _connectionCounterSubject.OnNext(_rosTopicServers.Count);
                                lazyDisposable.Dispose();
                            }
                        }
                        //_aggregateSubject.OnCompleted();
                    });

                lazyDisposable.Disposable = Disposable.Create(() =>
                {
                    d.Dispose();
                    lock (_disposables)
                    {
                        _disposables.Remove(d);
                    }
                });

                lock (_disposables)
                {
                    _disposables.Add(d);
                }
                lock (_rosTopicServers)
                {
                    _connectionCounterSubject.OnNext(_rosTopicServers.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("ConnectServer Error", ex);
            }
        }

        public IObservable<int> ConnectionChangedAsObservable()
        {
            return _connectionCounterSubject;
        }

        public void WaitForConnection()
        {
            ConnectionChangedAsObservable().Where(x => x > 0).Wait();
        }

        public void WaitForDisconnection()
        {
            ConnectionChangedAsObservable().Where(x => x == 0).Wait();
        }

        public void WaitForConnection(TimeSpan timeout)
        {
            ConnectionChangedAsObservable().Where(x => x > 0).Timeout(timeout).Wait();
        }

        public void WaitForDisconnection(TimeSpan timeout)
        {
            ConnectionChangedAsObservable().Where(x => x == 0).Timeout(timeout).Wait();
        }
    }
}